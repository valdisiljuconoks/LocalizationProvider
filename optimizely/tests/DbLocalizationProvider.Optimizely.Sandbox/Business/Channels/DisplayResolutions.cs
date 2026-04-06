using EPiServer.Framework.Localization;

namespace DbLocalizationProvider.Optimizely.Sandbox.Business.Channels;

/// <summary>
/// Defines resolution for desktop displays
/// </summary>
public class StandardResolution(LocalizationService localizationService)
    : DisplayResolutionBase(localizationService, "/resolutions/standard", 1366, 768)
{
}

/// <summary>
/// Defines resolution for a horizontal iPad
/// </summary>
public class IpadHorizontalResolution(LocalizationService localizationService)
    : DisplayResolutionBase(localizationService, "/resolutions/ipadhorizontal", 1024, 768)
{
}

/// <summary>
/// Defines resolution for a vertical iPhone 5s
/// </summary>
public class IphoneVerticalResolution(LocalizationService localizationService)
    : DisplayResolutionBase(localizationService, "/resolutions/iphonevertical", 320, 568)
{
}

/// <summary>
/// Defines resolution for a vertical Android handheld device
/// </summary>
public class AndroidVerticalResolution(LocalizationService localizationService)
    : DisplayResolutionBase(localizationService, "/resolutions/androidvertical", 480, 800)
{
}
