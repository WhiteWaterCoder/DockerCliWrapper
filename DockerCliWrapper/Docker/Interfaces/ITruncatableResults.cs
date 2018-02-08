namespace DockerCliWrapper.Docker.Interfaces
{
    public interface ITruncatableResults<T>
    {
        /// <summary>
        /// Set a flag denoting if the results should be truncated or not.
        /// </summary>
        /// <param name="doNotTruncate">Do not truncate flag.</param>
        /// <returns>The current instance (fluent interface).</returns>
        T DoNotTruncate(bool doTruncate);
    }
}