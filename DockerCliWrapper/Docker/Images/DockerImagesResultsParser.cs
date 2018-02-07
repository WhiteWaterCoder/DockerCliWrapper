using DockerCliWrapper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DockerCliWrapper.Docker.Images
{
    internal class DockerImagesResultsParser : ResultsParser
    {
        public static List<DockerImagesResult> ParseResult(string output)
        {
            var result = new List<DockerImagesResult>();

            // By default the following columns are returned:
            // REPOSITORY, TAG, IMAGE ID, CREATED, SIZE
            var lines = output.SplitLines();

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