using DockerCliWrapper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DockerCliWrapper.Docker.Image
{
    internal class DockerImageHistoryResultsParser : ResultsParser
    {
        public static List<DockerImageHistoryResult> ParseResult(string output, bool isHumanReadable)
        {
            var result = new List<DockerImageHistoryResult>();

            // By default the following columns are returned:
            // IMAGE, CREATED (AT), CREATED BY, SIZE, COMMENT
            var lines = output.SplitLines();

            // Spacing is adjusted according to results so need to split row based on calculated lengths
            var headerLine = lines.First();

            headerLine = headerLine.TrimStart().Replace("IMAGE", "");
            int imageColumnLength = "IMAGE".Length + headerLine.TakeWhile(Char.IsWhiteSpace).Count();

            string createdColumnHeader = isHumanReadable ? "CREATED" : "CREATED AT";
            var regex = new Regex(Regex.Escape(createdColumnHeader));
            headerLine = regex.Replace(headerLine.TrimStart(), "", 1);
            int createdColumnLength = createdColumnHeader.Length + headerLine.TakeWhile(Char.IsWhiteSpace).Count();

            headerLine = headerLine.TrimStart().Replace("CREATED BY", "");
            int createdByColumnLength = "CREATED BY".Length + headerLine.TakeWhile(Char.IsWhiteSpace).Count();

            headerLine = headerLine.TrimStart().Replace("SIZE", "");
            int sizeColumnLength = "SIZE".Length + headerLine.TakeWhile(Char.IsWhiteSpace).Count();

            headerLine = headerLine.TrimStart().Replace("COMMENT", "");
            int commentColumnLength = "COMMENT".Length + headerLine.TakeWhile(Char.IsWhiteSpace).Count();

            // The first line contains the header names 
            foreach (var line in lines.Skip(1))
            {
                string image = line.Substring(0, imageColumnLength).Trim();
                string created = isHumanReadable ? line.Substring(imageColumnLength, createdColumnLength).Trim() : "";
                DateTime.TryParse(line.Substring(imageColumnLength, createdColumnLength).Trim(), out DateTime createdAt);
                string createdBy = line.Substring(imageColumnLength + createdColumnLength, createdByColumnLength).Trim();
                string size = line.Substring(imageColumnLength + createdColumnLength + createdByColumnLength, sizeColumnLength).Trim();
                string comment = line.Substring(imageColumnLength + createdColumnLength + createdByColumnLength + sizeColumnLength).Trim();

                result.Add(new DockerImageHistoryResult(image, created, createdAt, createdBy, size, comment));
            }

            return result;
        }
    }
}