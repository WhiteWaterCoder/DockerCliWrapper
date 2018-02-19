namespace DockerCliWrapper.Docker.Interfaces
{
    /// <summary>
    /// Results that return their data truncated by default but can be changed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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