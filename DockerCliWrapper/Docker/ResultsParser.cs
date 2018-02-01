using System;
using System.Collections.Generic;
using System.Linq;

namespace DockerCliWrapper.Docker
{
    internal class ResultsParser
    {
        protected static readonly char[] _newLineSplitter = new[] { '\r', '\n' };
        private static readonly string[] _formattedResultsSplitter = new[] { " ~ " };

        public static List<T> ParseQuietResult<T>(string output, Func<string, T> factory)
            where T : IResult
        {
            // We only get a list of IMAGE IDs without a header
            return output.Split(_newLineSplitter, StringSplitOptions.RemoveEmptyEntries)
                         .Select(i => factory(i))
                         .ToList();
        }

        public static List<T> ParseFormattedResult<T>(string output, 
            Func<string, string, string, string, string, DateTime, string, T> factory,
            List<GoFormattingPlaceHolders> placeHolders)
        {
            var result = new List<T>();

            // We will get no header and columns in specified order, seperated by ~
            var lines = output.Split(_newLineSplitter, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
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

                foreach (var column in columns)
                {
                    if (placeHolders[columnIndex] == GoFormattingPlaceHolders.ImageId)
                    {
                        imageId = column;
                        continue;
                    }
                    if (placeHolders[columnIndex] == GoFormattingPlaceHolders.ImageRepository)
                    {
                        repository = column;
                        continue;
                    }
                    if (placeHolders[columnIndex] == GoFormattingPlaceHolders.ImageTag)
                    {
                        tag = column;
                        continue;
                    }
                    if (placeHolders[columnIndex] == GoFormattingPlaceHolders.ImageDigest)
                    {
                        digest = column;
                        continue;
                    }
                    if (placeHolders[columnIndex] == GoFormattingPlaceHolders.ElapsedTimeSinceImageWasCreated)
                    {
                        createdSince = column;
                        continue;
                    }
                    if (placeHolders[columnIndex] == GoFormattingPlaceHolders.ImageDiskSize)
                    {
                        size = column;
                        continue;
                    }
                    if (placeHolders[columnIndex] == GoFormattingPlaceHolders.TimeWhenTheImageWasCreated)
                    {
                        DateTime.TryParse(column, out createdAt);
                        continue;
                    }
                }
                
                result.Add(factory(imageId, repository, tag, digest, createdSince, createdAt, size));
            }

            return result;
        }
    }
}