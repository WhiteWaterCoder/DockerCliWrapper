using System.Collections.Generic;

namespace DockerCliWrapper.Docker.Container
{
    public class DockerContainersResult : IResult
    {
        public string Id { get; }
        public string Image { get; }
        public string Command { get; }
        public string Created { get; }
        public string Status { get; }
        public string Ports { get; }
        public string Names { get; }
        public string Size { get; }

        public DockerContainersResult(string id)
        {
            Id = id;
        }

        public DockerContainersResult(string id, string image, string command, string created, string status, string ports, string names, string size)
        {
            Id = id;
            Image = image;
            Command = command;
            Created = created;
            Status = status;
            Ports = ports;
            Names = names;
            Size = size;
        }

        public override string ToString()
        {
            return $"{Image}-{Id}";
        }

        public override bool Equals(object obj)
        {
            var result = obj as DockerContainersResult;
            return result != null &&
                   Id == result.Id &&
                   Image == result.Image &&
                   Command == result.Command &&
                   Created == result.Created &&
                   Status == result.Status &&
                   Ports == result.Ports &&
                   Names == result.Names &&
                   Size == result.Size;
        }

        public override int GetHashCode()
        {
            var hashCode = -1204735920;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Image);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Command);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Created);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Status);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Ports);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Names);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Size);
            return hashCode;
        }
    }
}