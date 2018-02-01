using DockerCliWrapper.Docker;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DockerCliWrapper.Extensions
{
    public static class StringBuilderArgumentExtensions
    {
        public static void AppendGoFormattingArguments(this StringBuilder sb, List<GoFormattingPlaceHolders> placeHolders)
        {
            if (placeHolders.Any())
            {
                sb.AppendFormat(
                    " --format \"{0}\"",
                    string.Join(" ~ ", placeHolders.Select(p => "{{" + p.GetDescription() + "}}")));
            }
        }

        public static void AppendNoTruncArgument(this StringBuilder sb, bool doNotTruncate)
        {
            if (doNotTruncate)
            {
                sb.Append(" --no-trunc");
            }
        }

        public static void AppendQuietArgument(this StringBuilder sb, bool beQuiet)
        {
            if (beQuiet)
            {
                sb.Append(" -q");
            }
        }
    }
}