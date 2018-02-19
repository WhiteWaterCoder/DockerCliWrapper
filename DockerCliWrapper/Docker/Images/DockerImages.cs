using DockerCliWrapper.Extensions;
using DockerCliWrapper.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DockerCliWrapper.Docker.Images
{
    /// <summary>
    /// Read operation for all local images. <see cref="https://docs.docker.com/engine/reference/commandline/images/"/> 
    /// for full CLI details.
    /// </summary>
    public class DockerImages : ExtendedSearchableBase<DockerImages, DockerImagesResult>
    {        
        private bool _showDigests;
        private List<GoFormattingPlaceHolders> _placeHolders;
        private string _repository;
        private string _tag;

        public DockerImages()
            : this(ShellExecutor.Instance)
        {
        }

        internal DockerImages(IShellExecutor shellExecutor)
            : base(shellExecutor)
        {
            _filters = new Dictionary<string, string>();
            _placeHolders = new List<GoFormattingPlaceHolders>();
        }

        /// <summary>
        /// Set a flag to denote if digests data should be returned.
        /// </summary>
        /// <param name="showDigests">ShowDigests flag.</param>
        /// <returns>The current instance (fluent interface).</returns>
        public DockerImages ShowDigests(bool showDigests)
        {
            _showDigests = showDigests;
            return this;
        }
        
        /// <summary>
        /// Pretty-print images using a Go template.
        /// </summary>
        /// <param name="placeHolders">A collection of enums containing the available Go placeholder.</param>
        /// <returns>The current instance (fluent interface).</returns>
        public DockerImages FormatResults(List<GoFormattingPlaceHolders> placeHolders)
        {
            if (!placeHolders.Any())
            {
                throw new ArgumentException("At least one placeholder has to be defined.", nameof(placeHolders));
            }

            _placeHolders = placeHolders;
            return this;
        }
        
        /// <summary>
        /// Restricts the list to all images that match the argument.
        /// </summary>
        /// <param name="repository">The repository filter.</param>
        /// <returns>The current instance (fluent interface).</returns>
        public DockerImages ForRepository(string repository)
        {
            _repository = repository;
            return this;
        }

        /// <summary>
        /// Restricts the list to images that match the argument and TAG.
        /// </summary>
        /// <param name="repository">The repository filter.</param>
        /// <param name="tag">The tag filter.</param>
        /// <returns>The current instance (fluent interface).</returns>
        public DockerImages ForRepositoryAndTag(string repository, string tag)
        {
            _repository = repository;
            _tag = tag;
            return this;
        }

        protected override List<DockerImagesResult> DoSearch(string output)
        {
            if (_beQuiet)
            {
                return ResultsParser.ParseQuietResult(output, i => new DockerImagesResult(i));
            }

            if (_placeHolders.Any())
            {
                return ResultsParser.ParseFormattedResult(output, 
                    (imageId, repository, tag, digest, createdSince, createdAt, size) => new DockerImagesResult(imageId, repository, tag, digest, createdSince, createdAt, size),
                    _placeHolders);
            }

            return DockerImagesResultsParser.ParseResult(output);
        }

        protected override string DefaultArg => " images";

        protected override DockerImages GetThis()
        {
            return this;
        }

        protected override void AppendArguments(StringBuilder arguments)
        {
            base.AppendArguments(arguments);

            if (_showDigests)
            {
                arguments.Append(" -digests");
            }

            arguments.AppendGoFormattingArguments(_placeHolders);
        }
    }
}