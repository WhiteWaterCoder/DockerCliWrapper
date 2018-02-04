using DockerCliWrapper.Infrastructure;

namespace DockerCliWrapper.Docker.Image
{
    /// <summary>
    /// Management (create/delete etc) operations for local images. <see cref="https://docs.docker.com/engine/reference/commandline/image/"/> 
    /// for full CLI details.
    /// </summary>
    public class DockerImage
    {
        internal const string Command = "docker";
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
        /// <param name="errorMessage">The error message if one occurs.</param>
        /// <returns>True if the image was deleted successfully, otherwise false.</returns>
        public bool Remove(out string errorMessage)
        {
            return Remove(false, out errorMessage);
        }

        /// <summary>
        /// Forcibly remove the image, even when a container based on the image exists,
        /// in which case the container will also be deleted.
        /// </summary>
        /// <param name="errorMessage">The error message if one occurs.</param>
        /// <returns>True if the image was deleted successfully, otherwise false.</returns>
        public bool ForceRemove(out string errorMessage)
        {
            return Remove(true, out errorMessage);
        }

        /// <summary>
        /// Creates an image history object which can have its settings set and executed.
        /// </summary>
        /// <returns>The history object for this image.</returns>
        public DockerImageHistory History()
        {
            return new DockerImageHistory(this, _shellExecutor);
        }

        private bool Remove(bool force, out string errorMessage)
        {
            var result = _shellExecutor.Execute(Command, $" {DefaultArg} {RemoveFlag} {ImageName}{(force ? " -f" : "" )}");

            if (result.IsSuccessFull)
            {
                errorMessage = "";
                return true;
            }

            errorMessage = result.Error;

            return false;
        }

        public override string ToString()
        {
            return ImageName;
        }
    }
}