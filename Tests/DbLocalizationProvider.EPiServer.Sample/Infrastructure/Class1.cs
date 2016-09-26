using System;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace DbLocalizationProvider.EPiServer.Sample.Infrastructure
{
    [LocalizedResource]
    public class ResolutionResources
    {
        [ResourceKey("/resolutions/androidvertical")]
        public static string AndroidVertical => "Android Vertical";

        [ResourceKey("/resolutions/androidvertical")]
        public static string AndroidVertical2 => "Android Vertical 2";
    }

    public class AndroidVerticalResolution : DisplayResolutionBase
    {
        public AndroidVerticalResolution() : base("/resolutions/androidvertical", 480, 800) { }
    }

    /// <summary>
    ///     Base class for all resolution definitions
    /// </summary>
    public abstract class DisplayResolutionBase : IDisplayResolution
    {
        protected DisplayResolutionBase(string name, int width, int height)
        {
            Id = GetType().FullName;
            Name = Translate(name);
            Width = width;
            Height = height;
        }

        private Injected<LocalizationService> LocalizationService { get; set; }

        /// <summary>
        ///     Gets the unique ID for this resolution
        /// </summary>
        public string Id { get; protected set; }

        /// <summary>
        ///     Gets the name of resolution
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        ///     Gets the resolution width in pixels
        /// </summary>
        public int Width { get; protected set; }

        /// <summary>
        ///     Gets the resolution height in pixels
        /// </summary>
        public int Height { get; protected set; }

        private string Translate(string resurceKey)
        {
            string value;

            try
            {
                if(!LocalizationService.Service.TryGetString(resurceKey, out value))
                {
                    value = resurceKey;
                }
            }
            catch (Exception)
            {
                value = resurceKey;
            }

            return value;
        }
    }
}
