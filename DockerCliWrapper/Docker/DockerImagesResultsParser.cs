using System;
using System.Collections.Generic;
using System.Linq;

namespace DockerCliWrapper.Docker
{
    internal static class DockerImagesResultsParser
    {
        private static readonly char[] _newLineSplitter = new[] { '\r', '\n' };
        private static readonly char[] _tabSplitter = new[] { '\t' };
        private static readonly string[] _formattedResultsSplitter = new[] { " ~ " };

        public static List<DockerImagesResult> ParseQuietResult(string output)
        {
            // We only get a list of IMAGE IDs without a header
            return output.Split(_newLineSplitter, StringSplitOptions.RemoveEmptyEntries)
                         .Select(i => new DockerImagesResult(i))
                         .ToList();
        }

        public static List<DockerImagesResult> ParseFormattedResult(string output, List<DockerImagesFormatPlaceHolders> placeHolders)
        {
            var result = new List<DockerImagesResult>();

            // We will get no header and columns in specified order, seperated by ~
            var lines = output.Split(_newLineSplitter, StringSplitOptions.RemoveEmptyEntries);

            foreach(var line in lines)
            {
                var columns = line.Split(_formattedResultsSplitter, StringSplitOptions.RemoveEmptyEntries);

                string imageId = "";
                string repository = "";
                string tag = "";
                string digest = "";
                string createdSince = "";
                string size = "";
                DateTime createdAt = DateTime.MinValue;

                int columnIndex = 0;

                foreach(var column in columns)
                {
                    if (placeHolders[columnIndex] == DockerImagesFormatPlaceHolders.ImageId)
                    {
                        imageId = column;
                        continue;
                    }
                    if (placeHolders[columnIndex] == DockerImagesFormatPlaceHolders.ImageRepository)
                    {
                        repository = column;
                        continue;
                    }
                    if (placeHolders[columnIndex] == DockerImagesFormatPlaceHolders.ImageTag)
                    {
                        tag = column;
                        continue;
                    }
                    if (placeHolders[columnIndex] == DockerImagesFormatPlaceHolders.ImageDigest)
                    {
                        digest = column;
                        continue;
                    }
                    if (placeHolders[columnIndex] == DockerImagesFormatPlaceHolders.ElapsedTimeSinceImageWasCreated)
                    {
                        createdSince = column;
                        continue;
                    }
                    if (placeHolders[columnIndex] == DockerImagesFormatPlaceHolders.ImageDiskSize)
                    {
                        size = column;
                        continue;
                    }
                    if (placeHolders[columnIndex] == DockerImagesFormatPlaceHolders.TimeWhenTheImageWasCreated)
                    {
                        DateTime.TryParse(column, out createdAt);
                        continue;
                    }
                }

                result.Add(new DockerImagesResult(imageId, repository, tag, digest, createdSince, createdAt, size));
            }

            return result;
        }

        public static List<DockerImagesResult> ParseResult(string output)
        {
            var result = new List<DockerImagesResult>();

            // By default the following columns are returned:
            // REPOSITORY, TAG, IMAGE ID, CREATED, SIZE
            var lines = output.Split(_newLineSplitter, StringSplitOptions.RemoveEmptyEntries);

            // Spacing is adjusted according to results so need to split row based on calculated lengths
            var headerLine = lines.First();

            headerLine = headerLine.TrimStart().Replace("REPOSITORY", "");
            int repositoryColumnLength = "REPOSITORY".Length + headerLine.TakeWhile(Char.IsWhiteSpace).Count();

            headerLine = headerLine.TrimStart().Replace("TAG", "");
            int tagColumnLength = "TAG".Length + headerLine.TakeWhile(Char.IsWhiteSpace).Count();

            headerLine = headerLine.TrimStart().Replace("IMAGE ID", "");
            int imageIdColumnLength = "IMAGE ID".Length + headerLine.TakeWhile(Char.IsWhiteSpace).Count();

            headerLine = headerLine.TrimStart().Replace("CREATED", "");
            int createdColumnLength = "CREATED".Length + headerLine.TakeWhile(Char.IsWhiteSpace).Count();

            // The first line contains the header names 
            foreach (var line in lines.Skip(1))
            {
                string repository = line.Substring(0, repositoryColumnLength).Trim();
                string tag = line.Substring(repositoryColumnLength, tagColumnLength).Trim();
                string imageId = line.Substring(repositoryColumnLength + tagColumnLength, imageIdColumnLength).Trim();
                string createdSince = line.Substring(repositoryColumnLength + tagColumnLength + imageIdColumnLength, createdColumnLength).Trim();
                string size = line.Substring(repositoryColumnLength + tagColumnLength + imageIdColumnLength + createdColumnLength).Trim();

                result.Add(new DockerImagesResult(imageId, repository, tag, "", createdSince, DateTime.MinValue, size));
            }

            return result;
        }
    }
}