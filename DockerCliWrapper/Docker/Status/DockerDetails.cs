using System.Collections.Generic;

namespace DockerCliWrapper.Docker.Status
{
    public class DockerDetails
    {
        public bool IsRunning { get; }
        public string Error { get; }
        public string Version { get; }
        public string ApiVersion { get; }
        public string GoVersion { get; }
        public string GitCommit { get; }
        public string Built { get; }
        public string OsArch { get; }
        public string Experimental { get; }

        public static DockerDetails CreateRunningStatus(string version, string apiVersion, string goVersion, string gitCommit, string built, string osArch, string experimental)
        {
            return new DockerDetails(true, "", version, apiVersion, goVersion, gitCommit, built, osArch, experimental);
        }

        public static DockerDetails CreateNonRunningStatus(string error)
        {
            return new DockerDetails(false, error, "", "", "", "", "", "", "");
        }

        private DockerDetails(bool isRunning, string error, string version, string apiVersion, string goVersion, string gitCommit, string built, string osArch, string experimental)
        {
            IsRunning = isRunning;
            Error = error;
            Version = version;
            ApiVersion = apiVersion;
            GoVersion = goVersion;
            GitCommit = gitCommit;
            Built = built;
            OsArch = osArch;
            Experimental = experimental;
        }

        public override bool Equals(object obj)
        {
            var details = obj as DockerDetails;
            return details != null &&
                   IsRunning == details.IsRunning &&
                   Error == details.Error &&
                   Version == details.Version &&
                   ApiVersion == details.ApiVersion &&
                   GoVersion == details.GoVersion &&
                   GitCommit == details.GitCommit &&
                   Built == details.Built &&
                   OsArch == details.OsArch &&
                   Experimental == details.Experimental;
        }

        public override int GetHashCode()
        {
            var hashCode = 1442701761;
            hashCode = hashCode * -1521134295 + IsRunning.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Error);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Version);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ApiVersion);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(GoVersion);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(GitCommit);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Built);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(OsArch);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Experimental);
            return hashCode;
        }
    }
}