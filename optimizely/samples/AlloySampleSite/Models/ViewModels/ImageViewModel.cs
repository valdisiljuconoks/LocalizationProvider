
namespace AlloySampleSite.Models.ViewModels
{
    /// <summary>
    /// View model for the image file
    /// </summary>
    public class ImageViewModel
    {
        /// <summary>
        /// Gets or sets the URL to the image.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the name of the image.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the copyright information of the image.
        /// </summary>
        public string Copyright { get; set; }
    }
}