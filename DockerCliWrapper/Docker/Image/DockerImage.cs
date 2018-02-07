using DockerCliWrapper.Docker.Constants;
using DockerCliWrapper.Infrastructure;
using System.Threading.Tasks;

namespace DockerCliWrapper.Docker.Image
{
    /// <summary>
    /// Management (create/delete etc) operations for local images. <see cref="https://docs.docker.com/engine/reference/commandline/image/"/> 
    /// for full CLI details.
    /// </summary>
    public class DockerImage
    {
        internal const string DefaultArg = "image";

        private const string RemoveFlag = "rm";

        private readonly IShellExecutor _shellExecutor;

        public string ImageName { get; }

        public DockerImage(string imageName)
            : this(imageName, ShellExecutor.Instance)
        {
        }

        internal DockerImage(string imageName, IShellExecutor shellExecutor)
        {
            Guard.IsNotNullOrEmpty(nameof(imageName), imageName);

            _shellExecutor = shellExecutor;

            ImageName = imageName;
        }

        /// <summary>
        /// Remove the image. If a container based on the image exists
        /// then it will not be removed.
        /// </summary>
        /// <returns>True if the image was deleted successfully, otherwise false along with the error message.</returns>
        public async Task<Result> Remove()
        {
            return await Remove(false);
        }

        /// <summary>
        /// Forcibly remove the image, even when a container based on the image exists,
        /// in which case the container will also be deleted.
        /// </summary>
        /// <returns>True if the image was deleted successfully, otherwise false along with the error message.</returns>
        public async Task<Result> ForceRemove()
        {
            return await Remove(true);
        }

        /// <summary>
        /// Creates an image history object which can have its settings set and executed.
        /// </summary>
        /// <returns>The history object for this image.</returns>
        public DockerImageHistory History()
        {
            return new DockerImageHistory(this, _shellExecutor);
        }

        private async Task<Result> Remove(bool force)
        {
            var shellResult = await _shellExecutor.Execute(Commands.Docker, $" {DefaultArg} {RemoveFlag} {ImageName}{(force ? " -f" : "" )}");

            return new Result(shellResult.IsSuccessFull, shellResult.Error);
        }

        public override string ToString()
        {
            return ImageName;
        }
    }
}