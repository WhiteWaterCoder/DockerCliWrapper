using System;

namespace DockerCliWrapper.Extensions
{
    public static class StringExtensions
    {
        private static readonly char[] _newLineSplitter = new[] { '\r', '\n' };

        public static string[] SplitLines(this string str)
        {
            return str.Split(_newLineSplitter, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}