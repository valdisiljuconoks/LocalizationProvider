using System.Collections.Generic;
using System.Threading.Tasks;
using AlloySampleSite.Infrastructure;
using AlloySampleSite.Models.FeedbackForm;
using AlloySampleSite.Models.Pages;
using EPiServer.Authorization;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlloySampleSite.Controllers;

public class FeedbackFormController : PageControllerBase<FeedbackFormPage>
{
    public const string ErrorKey = "CreateError";
    private readonly string AdminRoleName = Roles.WebAdmins;

    private UIUserProvider UIUserProvider => ServiceLocator.Current.GetInstance<UIUserProvider>();

    private UIRoleProvider UIRoleProvider => ServiceLocator.Current.GetInstance<UIRoleProvider>();

    private UISignInManager UISignInManager => ServiceLocator.Current.GetInstance<UISignInManager>();

    public IActionResult Index()
    {
        return View(new FeedbackFormViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryReleaseToken]
    public async Task<ActionResult> Index(FeedbackFormViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await UIUserProvider.CreateUserAsync(model.Username, model.Password, model.Email, null, null, true);
            if (result.Status == UIUserCreateStatus.Success)
            {
                await UIRoleProvider.CreateRoleAsync(AdminRoleName);
                await UIRoleProvider.AddUserToRolesAsync(result.User.Username, new[] { AdminRoleName });

                AdministratorRegistrationPageMiddleware.IsEnabled = false;
                SetFullAccessToWebAdmin();
                var resFromSignIn = await UISignInManager.SignInAsync(model.Username, model.Password);
                if (resFromSignIn)
                {
                    return Redirect("/");
                }
            }

            AddErrors(result.Errors);
        }

        // If we got this far, something failed, redisplay form
        return View(model);
    }

    private void SetFullAccessToWebAdmin()
    {
        var securityrep = ServiceLocator.Current.GetInstance<IContentSecurityRepository>();
        var permissions = securityrep.Get(ContentReference.RootPage).CreateWritableClone() as IContentSecurityDescriptor;
        permissions.AddEntry(new AccessControlEntry(AdminRoleName, AccessLevel.FullAccess));
        securityrep.Save(ContentReference.RootPage, permissions, SecuritySaveType.Replace);
    }

    private void AddErrors(IEnumerable<string> errors)
    {
        foreach (var error in errors)
        {
            ModelState.AddModelError(ErrorKey, error);
        }
    }
}
