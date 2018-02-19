using DockerCliWrapper.Infrastructure;
using System.Collections.Generic;
using System.Text;

namespace DockerCliWrapper.Docker.Container
{
    /// <summary>
    /// All bulk operation for local containers. <see cref="https://docs.docker.com/engine/reference/commandline/container/"/> 
    /// for full CLI details.
    /// </summary>
    public class DockerContainers : ExtendedSearchableBase<DockerContainers, DockerContainersResult>
    {
        private bool _showSizes;
        private int _numberOfLatestContainersToShow;
        private bool _showLatestContainer;

        public DockerContainers()
            : this(ShellExecutor.Instance)
        {
        }

        public DockerContainers(IShellExecutor shellExecutor)
            : base(shellExecutor)
        {
        }
        
        /// <summary>
        /// Set a flag to denote if the container sizes should be included in the resultset.
        /// </summary>
        /// <param name="showSize">ShowSize flag.</param>
        /// <returns>The current instance (fluent interface).</returns>
        public DockerContainers ShowSizes(bool showSizes)
        {
            _showSizes = showSizes;
            return this;
        }

        /// <summary>
        /// Set the number of latest containers to show. If set this will override the ShowLatestContainer
        /// and ShowAll flags.
        /// </summary>
        /// <param name="numberOfLatestContainersToShow"></param>
        /// <returns>The current instance (fluent interface).</returns>
        public DockerContainers ShowLatestContainers(int numberOfLatestContainersToShow)
        {
            _numberOfLatestContainersToShow = numberOfLatestContainersToShow;
            return this;
        }

        /// <summary>
        /// Set a flag to denote if only the latest container information should be returned.
        /// </summary>
        /// <param name="showLatestContainer">ShowLatestContainer flag</param>
        /// <returns>The current instance (fluent interface).</returns>
        public DockerContainers ShowLatestContainer(bool showLatestContainer)
        {
            _showLatestContainer = showLatestContainer;
            return this;
        }

        protected override List<DockerContainersResult> DoSearch(string output)
        {
            if (_beQuiet)
            {
                return ResultsParser.ParseQuietResult(output, i => new DockerContainersResult(i));
            }

            return DockerContainersResultsParser.ParseResult(output);
        }

        protected override string DefaultArg => " container ls";

        protected override DockerContainers GetThis()
        {
            return this;
        }

        protected override void AppendArguments(StringBuilder arguments)
        {
            base.AppendArguments(arguments);

            if (_showSizes)
            {
                arguments.Append(" -s");
            }

            if (_numberOfLatestContainersToShow > 0)
            {
                arguments.AppendFormat(" -n {0}", _numberOfLatestContainersToShow);
                return;
            }

            if (_showLatestContainer)
            {
                arguments.Append(" -l");
            }
        }
    }
}