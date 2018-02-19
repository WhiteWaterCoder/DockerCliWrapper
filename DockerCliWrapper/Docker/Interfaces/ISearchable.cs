using System.Collections.Generic;
using System.Threading.Tasks;

namespace DockerCliWrapper.Docker.Interfaces
{
    /// <summary>
    /// Defines an entity which can be search for.
    /// </summary>
    /// <typeparam name="TResult">The type of result object returned.</typeparam>
    public interface ISearchable<TResult>
    {
        /// <summary>
        /// Perform a search based on the state defined within this object.
        /// </summary>
        /// <returns></returns>
        Task<List<TResult>> SearchAsync();
    }
}