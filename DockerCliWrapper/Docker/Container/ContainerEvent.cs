using System.Collections.Generic;

namespace DockerCliWrapper.Docker.Container
{
    public class ContainerEvent
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

        public ContainerEventStatus EventStatus
        {
            get
            {
                switch(status)
                {
                    case "attach": return ContainerEventStatus.Attach;
                    case "commit": return ContainerEventStatus.Commit;
                    case "copy": return ContainerEventStatus.Copy;
                    case "create": return ContainerEventStatus.Create;
                    case "destroy": return ContainerEventStatus.Destroy;
                    case "detach": return ContainerEventStatus.Detach;
                    case "die": return ContainerEventStatus.Die;
                    case "exec_create": return ContainerEventStatus.ExecCreate;
                    case "exec_detach": return ContainerEventStatus.ExecDetach;
                    case "exec_start": return ContainerEventStatus.ExecStart;
                    case "export": return ContainerEventStatus.Export;
                    case "health_status": return ContainerEventStatus.HealthStatus;
                    case "kill": return ContainerEventStatus.Kill;
                    case "oom": return ContainerEventStatus.Oom;
                    case "pause": return ContainerEventStatus.Pause;
                    case "rename": return ContainerEventStatus.Rename;
                    case "resize": return ContainerEventStatus.Resize;
                    case "restart": return ContainerEventStatus.Restart;
                    case "start": return ContainerEventStatus.Start;
                    case "stop": return ContainerEventStatus.Stop;
                    case "top": return ContainerEventStatus.Top;
                    case "unpause": return ContainerEventStatus.Unpause;
                    case "update": return ContainerEventStatus.Update;
                    default: return ContainerEventStatus.Unknown;
                }
            }
        }

        public ContainerEvent(string status, string id, string from, string type)
        {
            this.status = status;
            this.id = id;
            this.from = from;
            this.Type = type;
        }

        public override string ToString()
        {
            return from;
        }

        public override bool Equals(object obj)
        {
            var @event = obj as ContainerEvent;
            return @event != null &&
                   status == @event.status &&
                   id == @event.id &&
                   from == @event.from &&
                   Type == @event.Type &&
                   ShortId == @event.ShortId;
        }

        public override int GetHashCode()
        {
            var hashCode = -1797317479;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(status);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(from);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ShortId);
            return hashCode;
        }
    }
}