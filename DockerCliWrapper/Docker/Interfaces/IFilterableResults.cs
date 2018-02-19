using System.Collections.Generic;

namespace DockerCliWrapper.Docker.Interfaces
{
    /// <summary>
    /// Results that can be filtered if required.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFilterableResults<T>
    {
        /// <summary>
        /// Set a filter to be applied on the resultset.
        /// </summary>
        /// <param name="filter">Filter column name.</param>
        /// <param name="value">Filter value.</param>
        /// <returns>The current instance (fluent interface).</returns>
        T SetFilter(string filter, string value);
    }
}