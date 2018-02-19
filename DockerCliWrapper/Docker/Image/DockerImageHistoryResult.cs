using System;
using System.Collections.Generic;

namespace DockerCliWrapper.Docker.Image
{
    public class DockerImageHistoryResult : IResult
    {
        public string Id { get; }
        public string CreatedSince { get; }
        public DateTime CreatedAt { get; }
        public string CreatedBy { get; }
        public string Size { get; }
        public string Comment { get; }

        public DockerImageHistoryResult(string id)
        {
            Id = id;
        }

        public DockerImageHistoryResult(string id, string createdSince, DateTime createdAt, string createdBy, string size, string comment) 
            : this(id)
        {
            CreatedSince = createdSince;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            Size = size;
            Comment = comment;
        }

        public override bool Equals(object obj)
        {
            var result = obj as DockerImageHistoryResult;
            return result != null &&
                   Id == result.Id &&
                   CreatedSince == result.CreatedSince &&
                   CreatedAt == result.CreatedAt &&
                   Size == result.Size &&
                   Comment == result.Comment;
        }

        public override int GetHashCode()
        {
            var hashCode = -96956286;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CreatedSince);
            hashCode = hashCode * -1521134295 + CreatedAt.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Size);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Comment);
            return hashCode;
        }

        public override string ToString()
        {
            return Id;
        }
    }
}