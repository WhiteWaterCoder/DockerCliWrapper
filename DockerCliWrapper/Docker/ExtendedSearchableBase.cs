using DockerCliWrapper.Docker.Interfaces;
using DockerCliWrapper.Infrastructure;
using System.Collections.Generic;
using System.Text;

namespace DockerCliWrapper.Docker
{
    public abstract class ExtendedSearchableBase<T, TResult> : SearchableBase<T, TResult>,
        IShowAllResults<T>,
        IFilterableResults<T>
    {
        protected bool _showAll;
        protected IDictionary<string, string> _filters;

        protected ExtendedSearchableBase(IShellExecutor shellExecutor)
            : base(shellExecutor)
        {
            _filters = new Dictionary<string, string>();
        }

        #region IShowAllResults<T> implementation

        /// <summary>
        /// Set a flag to denote if all results should be shown.
        /// </summary>
        /// <param name="showAll">ShowAll flag.</param>
        /// <returns>The current instance (fluent interface).</returns>
        public T ShowAll(bool showAll)
        {
            _showAll = showAll;
            return GetThis();
        }

        #endregion

        #region IFilterableResults<T> implementation

        /// <summary>
        /// Set a filter to be applied on the resultset.
        /// </summary>
        /// <param name="filter">Filter column name.</param>
        /// <param name="value">Filter value.</param>
        /// <returns>The current instance (fluent interface).</returns>
        public T SetFilter(string filter, string value)
        {
            _filters.Add(filter, value);
            return GetThis();
        }

        #endregion

        protected override void AppendArguments(StringBuilder arguments)
        {
            if (_showAll)
            {
                arguments.Append(" -a");
            }

            foreach (var filter in _filters)
            {
                arguments.AppendFormat(" -f \"{0}={1}\"", filter.Key, filter.Value);
            }
        }
    }
}