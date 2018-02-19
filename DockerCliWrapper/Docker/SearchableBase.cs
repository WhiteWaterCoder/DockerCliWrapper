using DockerCliWrapper.Docker.Constants;
using DockerCliWrapper.Docker.Interfaces;
using DockerCliWrapper.Extensions;
using DockerCliWrapper.Infrastructure;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DockerCliWrapper.Docker
{
    public abstract class SearchableBase<T, TResult> :
        ITruncatableResults<T>,
        IQuietResults<T>,
        ISearchable<TResult>
    {
        protected readonly IShellExecutor _shellExecutor;

        protected bool _doNotTruncate;
        protected bool _beQuiet;

        public SearchableBase(IShellExecutor shellExecutor)
        {
            _shellExecutor = shellExecutor;
        }
        
        #region ITruncatableResults<T> implementation

        /// <summary>
        /// Set a flag denoting if the results should be truncated or not.
        /// </summary>
        /// <param name="doNotTruncate">Do not truncate flag.</param>
        /// <returns>The current instance (fluent interface).</returns>
        public T DoNotTruncate(bool doNotTruncate)
        {
            _doNotTruncate = doNotTruncate;
            return GetThis();
        }

        #endregion

        #region IQuietResults<T> implementation

        /// <summary>
        /// Set a flag to denote if only IDs should be populated in the response.
        /// </summary>
        /// <param name="beQuiet">BeQuiet flag.</param>
        /// <returns>The current instance (fluent interface).</returns>
        public T BeQuiet(bool beQuiet)
        {
            _beQuiet = beQuiet;
            return GetThis();
        }

        #endregion
        
        #region ISearchable<TResult> implementation

        public async Task<List<TResult>> SearchAsync()
        {
            var result = await _shellExecutor.Execute(Commands.Docker, GenerateArguments());

            if (!result.IsSuccessFull)
            {
                //TODO: Log
                return new List<TResult>();
            }

            return DoSearch(result.Output);
        }

        protected abstract List<TResult> DoSearch(string output);

        #endregion

        protected abstract T GetThis();

        protected abstract string DefaultArg { get; }

        protected string GenerateArguments()
        {
            var arguments = new StringBuilder();

            arguments.Append(DefaultArg);
            
            arguments.AppendNoTruncArgument(_doNotTruncate);
            arguments.AppendQuietArgument(_beQuiet);

            AppendArguments(arguments);

            return arguments.ToString();
        }

        protected abstract void AppendArguments(StringBuilder arguments);
    }
}