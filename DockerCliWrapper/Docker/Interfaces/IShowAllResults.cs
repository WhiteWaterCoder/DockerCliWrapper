namespace DockerCliWrapper.Docker.Interfaces
{
    /// <summary>
    /// Results that return only a subset by default but can be setto return all.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IShowAllResults<T>
    {
        /// <summary>
        /// Set a flag to denote if all results should be displayed instead of just the default ones.
        /// </summary>
        /// <param name="showAll">ShowAll flag.</param>
        /// <returns>The current instance (fluent interface).</returns>
        T ShowAll(bool showAll);
    }
}