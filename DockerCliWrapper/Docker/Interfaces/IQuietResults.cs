namespace DockerCliWrapper.Docker.Interfaces
{
    public interface IQuietResults<T>
    {
        /// <summary>
        /// Set a flag to denote if only image IDs should be populated in the response.
        /// </summary>
        /// <param name="beQuiet">BeQuiet flag.</param>
        /// <returns>The current instance (fluent interface).</returns>
        T BeQuiet(bool beQuiet);
    }
}
