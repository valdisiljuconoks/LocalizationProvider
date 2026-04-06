using EPiServer.Framework.Blobs;
using EPiServer.Framework.DataAnnotations;

namespace DbLocalizationProvider.Optimizely.Sandbox.Models.Media;

[ContentType(GUID = "F522B459-EB27-462C-B216-989FC7FF9448")]
[MediaDescriptor(ExtensionString = "svg")]
public class VectorImageFile : ImageData
{
    /// <summary>
    /// Gets the generated thumbnail for this media.
    /// </summary>
    public override Blob Thumbnail => BinaryData;
}
