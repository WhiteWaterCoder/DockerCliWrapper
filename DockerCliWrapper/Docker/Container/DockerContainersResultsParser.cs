using DockerCliWrapper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DockerCliWrapper.Docker.Container
{
    internal class DockerContainersResultsParser : ResultsParser
    {
        public static List<DockerContainersResult> ParseResult(string output)
        {
            var result = new List<DockerContainersResult>();

            // By default the following columns are returned:
            // CONTAINER ID, IMAGE, COMMAND, CREATED, STATUS, PORTS, NAMES. SIZE is optional but always at the end
            var lines = output.SplitLines();

            // Spacing is adjusted according to results so need to split row based on calculated lengths
            var headerLine = lines.First();

            bool containsSizeColumn = headerLine.Contains("SIZE");

            headerLine = headerLine.TrimStart().Replace("CONTAINER ID", "");
            int containerIdColumnLength = "CONTAINER ID".Length + headerLine.TakeWhile(Char.IsWhiteSpace).Count();

            headerLine = headerLine.TrimStart().Replace("IMAGE", "");
            int imageColumnLength = "IMAGE".Length + headerLine.TakeWhile(Char.IsWhiteSpace).Count();

            headerLine = headerLine.TrimStart().Replace("COMMAND", "");
            int commandColumnLength = "COMMAND".Length + headerLine.TakeWhile(Char.IsWhiteSpace).Count();

            headerLine = headerLine.TrimStart().Replace("CREATED", "");
            int createdColumnLength = "CREATED".Length + headerLine.TakeWhile(Char.IsWhiteSpace).Count();

            headerLine = headerLine.TrimStart().Replace("STATUS", "");
            int statusColumnLength = "STATUS".Length + headerLine.TakeWhile(Char.IsWhiteSpace).Count();

            headerLine = headerLine.TrimStart().Replace("PORTS", "");
            int portsColumnLength = "PORTS".Length + headerLine.TakeWhile(Char.IsWhiteSpace).Count();

            headerLine = headerLine.TrimStart().Replace("NAMES", "");
            int namesColumnLength = "NAMES".Length + headerLine.TakeWhile(Char.IsWhiteSpace).Count();

            headerLine = headerLine.TrimStart().Replace("SIZE", "");
            int sizeColumnLength = "SIZE".Length + headerLine.TakeWhile(Char.IsWhiteSpace).Count();

            // The first line contains the header names 
            foreach (var line in lines.Skip(1))
            {
                string containerId = line.Substring(0, containerIdColumnLength).Trim();
                string image = line.Substring(containerIdColumnLength, imageColumnLength).Trim();
                string command = line.Substring(containerIdColumnLength + imageColumnLength, commandColumnLength).Trim();
                string created = line.Substring(containerIdColumnLength + imageColumnLength + commandColumnLength, createdColumnLength).Trim();
                string status = line.Substring(containerIdColumnLength + imageColumnLength + commandColumnLength + createdColumnLength, statusColumnLength).Trim();
                string ports = line.Substring(containerIdColumnLength + imageColumnLength + commandColumnLength + createdColumnLength + statusColumnLength, portsColumnLength).Trim();

                string names, size = "";

                if (containsSizeColumn)
                {
                    names = line.Substring(containerIdColumnLength + imageColumnLength + commandColumnLength + createdColumnLength + statusColumnLength + portsColumnLength, namesColumnLength).Trim();
                    size = line.Substring(containerIdColumnLength + imageColumnLength + commandColumnLength + createdColumnLength + statusColumnLength + portsColumnLength + namesColumnLength).Trim();
                }
                else
                {
                    names = line.Substring(containerIdColumnLength + imageColumnLength + commandColumnLength + createdColumnLength + statusColumnLength + portsColumnLength).Trim();
                }

                result.Add(new DockerContainersResult(containerId, image, command, created, status, ports, names, size));
            }

            return result;
        }
    }
}