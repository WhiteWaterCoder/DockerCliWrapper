using DockerCliWrapper.Extensions;
using DockerCliWrapper.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DockerCliWrapper.Docker.Image
{
    public class DockerImageHistory : SearchableBase<DockerImageHistory, DockerImageHistoryResult>
    {
        private readonly DockerImage _image;

        private List<GoFormattingPlaceHolders> _placeHolders;
        private bool _humanReadable = true;

        internal DockerImageHistory(DockerImage image, IShellExecutor shellExecutor)
            : base(shellExecutor)
        {
            _image = image;

            _placeHolders = new List<GoFormattingPlaceHolders>();
        }

        /// <summary>
        /// Pretty-print images using a Go template.
        /// </summary>
        /// <param name="placeHolders">A collection of enums containing the available Go placeholder.</param>
        /// <returns>The current instance (fluent interface).</returns>
        public DockerImageHistory FormatResults(List<GoFormattingPlaceHolders> placeHolders)
        {
            if (!placeHolders.Any())
            {
                throw new ArgumentException("At least one placeholder has to be defined.", nameof(placeHolders));
            }

            _placeHolders = placeHolders;
            return this;
        }

        /// <summary>
        /// Print sizes and dates in human readable format. Default is true.
        /// </summary>
        /// <param name="humanReadable">Flag to denote if output (sizes and dates) should be in human readable format.</param>
        /// <returns>The current instance (fluent interface).</returns>
        public DockerImageHistory CreateOutputInHumanReadableFormat(bool humanReadable)
        {
            _humanReadable = humanReadable;
            return this;
        }

        protected override DockerImageHistory GetThis()
        {
            return this;
        }

        protected override string DefaultArg => $" {DockerImage.DefaultArg} history";

        protected override List<DockerImageHistoryResult> DoSearch(string output)
        {
            if (_beQuiet)
            {
                return ResultsParser.ParseQuietResult(output, i => new DockerImageHistoryResult(i));
            }

            if (_placeHolders.Any())
            {
                return ResultsParser.ParseFormattedResult(output,
                        (imageId, _, __, ___, createdSince, createdAt, size) => new DockerImageHistoryResult(imageId, createdSince, createdAt, "", size, ""),
                        _placeHolders);
            }

            return DockerImageHistoryResultsParser.ParseResult(output, _humanReadable);
        }

        protected override void AppendArguments(StringBuilder arguments)
        {
            arguments.AppendFormat($" -H={_humanReadable}");

            arguments.AppendGoFormattingArguments(_placeHolders);

            arguments.Append($" {_image}");
        }
    }
}