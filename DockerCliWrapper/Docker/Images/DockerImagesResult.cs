using System;
using System.Collections.Generic;

namespace DockerCliWrapper.Docker.Images
{
    public class DockerImagesResult
    {
        public string ImageId { get; }
        public string Repository { get; }
        public string Tag { get; }
        public string Digest { get; }
        public string CreatedSince { get; }
        public DateTime CreatedAt { get; }
        public string Size { get; }

        public DockerImagesResult(string imageId)
        {
            ImageId = imageId;
        }

        public DockerImagesResult(string imageId, string repository, string tag, string digest, string createdSince, DateTime createdAt, string size)
        {
            ImageId = imageId;
            Repository = repository;
            Tag = tag;
            Digest = digest;
            CreatedSince = createdSince;
            CreatedAt = createdAt;
            Size = size;
        }
        
        public override bool Equals(object obj)
        {
            var result = obj as DockerImagesResult;
            return result != null &&
                   ImageId == result.ImageId &&
                   Repository == result.Repository &&
                   Tag == result.Tag &&
                   Digest == result.Digest &&
                   CreatedSince == result.CreatedSince &&
                   CreatedAt == result.CreatedAt &&
                   Size == result.Size;
        }

        public override int GetHashCode()
        {
            var hashCode = -2026909135;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ImageId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Repository);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Tag);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Digest);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CreatedSince);
            hashCode = hashCode * -1521134295 + CreatedAt.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Size);
            return hashCode;
        }

        public override string ToString()
        {
            return $"{ImageId} - {Repository}";
        }
    }
}