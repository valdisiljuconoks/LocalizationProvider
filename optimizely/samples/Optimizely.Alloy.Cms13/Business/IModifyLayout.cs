using Optimizely.Alloy.Cms13.Models.ViewModels;

namespace Optimizely.Alloy.Cms13.Business;

/// <summary>
/// Defines a method which may be invoked by PageContextActionFilter allowing controllers
/// to modify common layout properties of the view model.
/// </summary>
internal interface IModifyLayout
{
    public void ModifyLayout(LayoutModel layoutModel);
}
