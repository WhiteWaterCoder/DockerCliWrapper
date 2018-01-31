using System.ComponentModel;

namespace DockerCliWrapper.Docker.Images
{
    public enum DockerImagesFormatPlaceHolders
    {
        [Description(".ID")]
        ImageId,
        [Description(".Repository")]
        ImageRepository,
        [Description(".Tag")]
        ImageTag,
        [Description(".Digest")]
        ImageDigest,
        [Description(".CreatedSince")]
        ElapsedTimeSinceImageWasCreated,
        [Description(".CreatedAt")]
        TimeWhenTheImageWasCreated,
        [Description(".Size")]
        ImageDiskSize
    }
}