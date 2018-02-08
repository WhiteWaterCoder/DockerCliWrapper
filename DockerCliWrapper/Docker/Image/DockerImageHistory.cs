﻿using DockerCliWrapper.Docker.Constants;
using DockerCliWrapper.Docker.Interfaces;
using DockerCliWrapper.Extensions;
using DockerCliWrapper.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockerCliWrapper.Docker.Image
{
    public class DockerImageHistory : 
        ITruncatableResults<DockerImageHistory>,
        IQuietResults<DockerImageHistory>
    {
        private readonly DockerImage _image;
        private readonly IShellExecutor _shellExecutor;

        private List<GoFormattingPlaceHolders> _placeHolders;
        private bool _humanReadable = true;
        private bool _doNotTruncate;
        private bool _beQuiet;

        internal DockerImageHistory(DockerImage image, IShellExecutor shellExecutor)
        {
            _image = image;
            _shellExecutor = shellExecutor;

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

        /// <summary>
        /// Set a flag denoting if the results should be truncated or not.
        /// </summary>
        /// <param name="doNotTruncate">Do not truncate flag.</param>
        /// <returns>The current instance (fluent interface).</returns>
        public DockerImageHistory DoNotTruncate(bool doNotTruncate)
        {
            _doNotTruncate = doNotTruncate;
            return this;
        }

        /// <summary>
        /// Set a flag to denote if only image IDs should be populated in the response.
        /// </summary>
        /// <param name="beQuiet">BeQuiet flag.</param>
        /// <returns>The current instance (fluent interface).</returns>
        public DockerImageHistory BeQuiet(bool beQuiet)
        {
            _beQuiet = beQuiet;
            return this;
        }

        /// <summary>
        /// Execute the object based on its current state.
        /// </summary>
        /// <returns>A list of docker image history entries that fulfill the criteria specified on the object.</returns>
        public async Task<List<DockerImageHistoryResult>> Execute()
        {
            var result = await _shellExecutor.Execute(Commands.Docker, GenerateArguments());

            if (!result.IsSuccessFull)
            {
                //TODO: Log
                return new List<DockerImageHistoryResult>();
            }

            if (_beQuiet)
            {
                return ResultsParser.ParseQuietResult(result.Output, i => new DockerImageHistoryResult(i));
            }

            if (_placeHolders.Any())
            {
                return ResultsParser.ParseFormattedResult(result.Output,
                        (imageId, _, __, ___, createdSince, createdAt, size) => new DockerImageHistoryResult(imageId, createdSince, createdAt, "", size, ""),
                        _placeHolders);
            }

            return DockerImageHistoryResultsParser.ParseResult(result.Output, _humanReadable);
        }

        private string GenerateArguments()
        {
            var arguments = new StringBuilder();

            arguments.Append($" {DockerImage.DefaultArg}");
            arguments.Append(" history");

            arguments.AppendFormat($" -H={_humanReadable}");
            
            arguments.AppendGoFormattingArguments(_placeHolders);
            arguments.AppendNoTruncArgument(_doNotTruncate);
            arguments.AppendQuietArgument(_beQuiet);

            arguments.Append($" {_image}");

            return arguments.ToString();
        }
    }
}