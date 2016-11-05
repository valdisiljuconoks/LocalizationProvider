using System.Web.Mvc;

namespace DbLocalizationProvider.AdminUI
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }

        public UiContextMode Mode { get; set; } = UiContextMode.None;

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // merge both roles into single list
            var admins = string.Join(",", UiConfigurationContext.Current.AuthorizedAdminRoles);
            var editors = string.Join(",", UiConfigurationContext.Current.AuthorizedEditorRoles);

            var rolesToCheck = string.Join(",", string.Join(",", admins, editors));
            switch (Mode)
            {
                case UiContextMode.Admin:
                    rolesToCheck = admins;
                    break;
                case UiContextMode.Edit:
                    rolesToCheck = editors;
                    break;
            }

            Roles = rolesToCheck;
            base.OnAuthorization(filterContext);
        }
    }
}
