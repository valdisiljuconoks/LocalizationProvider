using EPiServer.Framework.DataAnnotations;

namespace Optimizely.Alloy.Cms13.Models.Media;

[ContentType(GUID = "0A89E464-56D4-449F-AEA8-2BF774AB8730")]
[MediaDescriptor(ExtensionString = "jpg,jpeg,jpe,ico,gif,bmp,png")]
public class ImageFile : ImageData
{
    /// <summary>
    /// Gets or sets the copyright.
    /// </summary>
    /// <value>
    /// The copyright.
    /// </value>
    public virtual string Copyright { get; set; }
}
