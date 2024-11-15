using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace AlloySampleSite.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class RegisterFirstAdminWithLocalRequestAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (AdministratorRegistrationPageMiddleware.IsEnabled == false)
            {
                context.Result = new NotFoundResult();
                return;
            }
        }
    }
}
