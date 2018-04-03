using System.Collections.Generic;

namespace DockerCliWrapper.Docker.Image
{
    public class ImageEvent
    {
        public string status { get; }
        public string id { get; }
        public string from { get; }
        public string Type { get; }

        public string ShortId
        {
            get
            {
                return !string.IsNullOrEmpty(id) && id.Length > 12
                    ? id.Substring(0, 12)
                    : "";
            }
        }

        public ImageEventStatus EventStatus
        {
            get
            {
                switch (status)
                {
                    case "pull": return ImageEventStatus.Pull;
                    case "delete": return ImageEventStatus.Delete;
                    case "tag": return ImageEventStatus.Tag;
                    case "untag": return ImageEventStatus.Untag;
                    default: return ImageEventStatus.Unknown;
                }
            }
        }

        public ImageEvent(string status, string id, string from, string type)
        {
            this.status = status;
            this.id = id;
            this.from = from;
            Type = type;
        }

        public override bool Equals(object obj)
        {
            var @event = obj as ImageEvent;
            return @event != null &&
                   status == @event.status &&
                   id == @event.id &&
                   from == @event.from &&
                   Type == @event.Type &&
                   ShortId == @event.ShortId &&
                   EventStatus == @event.EventStatus;
        }

        public override int GetHashCode()
        {
            var hashCode = 1182173962;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(status);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(from);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ShortId);
            hashCode = hashCode * -1521134295 + EventStatus.GetHashCode();
            return hashCode;
        }
    }
}
