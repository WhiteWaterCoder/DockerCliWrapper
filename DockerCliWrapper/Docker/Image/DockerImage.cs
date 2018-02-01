using DockerCliWrapper.Infrastructure;

namespace DockerCliWrapper.Docker.Image
{
    /// <summary>
    /// Management (create/delete etc) operations for local images. <see cref="https://docs.docker.com/engine/reference/commandline/image/"/> 
    /// for full CLI details.
    /// </summary>
    public class DockerImage
    {
        private const string Command = "docker";
        private const string DefaultArg = "image";

        private const string RemoveFlag = "rm";

        private readonly string _imageName;

        public DockerImage(string imageName)
        {
            Guard.IsNotNullOrEmpty(nameof(imageName), _imageName);

            _imageName = imageName;
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

        private bool Remove(bool force, out string errorMessage)
        {
            var result = ShellExecutor.Instance.Execute(Command, $" {DefaultArg} {RemoveFlag} {_imageName}{(force ? " -f" : "" )}");

            if (result.IsSuccessFull)
            {
                errorMessage = "";
                return true;
            }

            errorMessage = result.Error;

            return false;
        }
    }
}