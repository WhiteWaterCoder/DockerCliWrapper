using DockerCliWrapper.Docker.Constants;
using DockerCliWrapper.Extensions;
using DockerCliWrapper.Infrastructure;
using System.Threading.Tasks;

namespace DockerCliWrapper.Docker.Status
{
    public class DockerStatus
    {
        private readonly IShellExecutor _shellExecutor;
        
        public DockerDetails ClientDetails { get; private set; }
        public DockerDetails ServerDetails { get; private set; }

        public DockerStatus()
            : this(ShellExecutor.Instance)
        {
        }

        public DockerStatus(IShellExecutor shellExecutor)
        {
            _shellExecutor = shellExecutor;
        }

        /// <summary>
        /// Connect to the docker daemon and try retrieve the client & server details.
        /// </summary>
        /// <returns>Nothing (populates instance properties)</returns>
        public async Task Connect()
        {
            var result = await _shellExecutor.Execute(Commands.Docker, "version");
            
            var lines = result.Output.SplitLines();

            bool isClient = false;
            string version = string.Empty;
            string apiVersion = string.Empty;
            string goVersion = string.Empty;
            string gitCommit = string.Empty;
            string built = string.Empty;
            string osArch = string.Empty;
            string isExperimental = string.Empty;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("Client:")) { isClient = true; }
                if (lines[i].Contains("Version:")) { version = lines[i].Replace("Version:", "").Trim(); }
                if (lines[i].Contains("API version:")) { apiVersion = lines[i].Replace("API version:", "").Trim(); }
                if (lines[i].Contains("Go version:")) { goVersion = lines[i].Replace("Go version:", "").Trim(); }
                if (lines[i].Contains("Git commit:")) { gitCommit = lines[i].Replace("Git commit:", "").Trim(); }
                if (lines[i].Contains("Built:")) { built = lines[i].Replace("Built:", "").Trim(); }
                if (lines[i].Contains("OS/Arch:")) { osArch = lines[i].Replace("OS/Arch:", "").Trim(); }
                if (lines[i].Contains("Experimental:")) { isExperimental = lines[i].Replace("Experimental:", "").Trim(); }

                if (lines[i].Contains("Server:") || i == lines.Length - 1)
                {
                    if (isClient)
                    {
                        ClientDetails = DockerDetails.CreateRunningStatus(version, apiVersion, goVersion, gitCommit, built, osArch, isExperimental);
                        isClient = false;
                        version = apiVersion = goVersion = gitCommit = built = osArch = isExperimental = string.Empty;
                    }
                    else
                    {
                        ServerDetails = DockerDetails.CreateRunningStatus(version, apiVersion, goVersion, gitCommit, built, osArch, isExperimental);
                        isClient = false;
                        version = apiVersion = goVersion = gitCommit = built = osArch = isExperimental = string.Empty;
                    }
                }
            }

            // If an error occurred it will be with the server so parse error output here
            if (!string.IsNullOrEmpty(result.Error))
            {
                ServerDetails = DockerDetails.CreateNonRunningStatus(result.Error);
            }
        }
    }
}