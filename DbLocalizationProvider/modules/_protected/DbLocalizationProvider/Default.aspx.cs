using System;
using EPiServer;
using EPiServer.Shell.WebForms;

namespace TechFellow.DbLocalizationProvider
{
    public partial class Default : WebFormsBase
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            MasterPageFile = UriSupport.ResolveUrlFromUIBySettings("MasterPages/Frameworks/Framework.Master");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Page.Header.DataBind();
        }
    }
}
