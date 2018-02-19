namespace DockerCliWrapper.Docker.Interfaces
{
    /// <summary>
    /// Results that return a whole set by default but can be set to return only their identifier instead.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
