using System.ComponentModel.DataAnnotations;

namespace Optimizely.Alloy.Cms13;

public class Globals
{
    public const string LoginPath = "/util/login";

    /// <summary>
    /// Group names for content types and properties
    /// </summary>
    [GroupDefinitions]
    public static class GroupNames
    {
        [Display(Name = "Default", Order = 10)]
        public const string Default = "Default";

        [Display(Name = SystemTabNames.Content, Order = 20)]
        public const string Content = SystemTabNames.Content;

        [Display(Name = "Contact", Order = 30)]
        public const string Contact = "Contact";

        [Display(Name = "Metadata", Order = 40)]
        public const string MetaData = "Metadata";

        [Display(Name = "News", Order = 50)]
        public const string News = "News";

        [Display(Name = "Products", Order = 60)]
        public const string Products = "Products";

        [Display(Name = SystemTabNames.Settings, Order = 70)]
        public const string Settings = SystemTabNames.Settings;

        [Display(Name = "SiteSettings", Order = 80)]
        public const string SiteSettings = "SiteSettings";

        [Display(Name = "Specialized", Order = 90)]
        public const string Specialized = "Specialized";
    }

    /// <summary>
    /// Tags to use for the main widths used in the Bootstrap HTML framework
    /// </summary>
    public static class ContentAreaTags
    {
        public const string FullWidth = "full";
        public const string WideWidth = "wide";
        public const string HalfWidth = "half";
        public const string NarrowWidth = "narrow";
        public const string NoRenderer = "norenderer";
    }

    /// <summary>
    /// Names used for UIHint attributes to map specific rendering controls to page properties
    /// </summary>
    public static class SiteUIHints
    {
        public const string Contact = "contact";
        public const string Strings = "StringList";
        public const string StringsCollection = "StringsCollection";
    }

    /// <summary>
    /// Virtual path to folder with static graphics, such as "/gfx/"
    /// </summary>
    public const string StaticGraphicsFolderPath = "/gfx/";
}
