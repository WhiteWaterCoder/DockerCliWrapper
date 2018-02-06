using DockerCliWrapper.Extensions;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DockerCliWrapper.Infrastructure
{
    public class ShellExecutor : IShellExecutor
    {
        private static readonly ShellExecutor _instance = new ShellExecutor();

        private readonly Process _process;

        private ShellExecutor()
        {
            _process = new Process();

            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardError = true;
        }

        public static IShellExecutor Instance => _instance;

        public async Task<ShellExecuteResult> Execute(string command, string arguments)
        {
            _process.StartInfo.FileName = command;
            _process.StartInfo.Arguments = arguments;
            _process.Start();
            
            string output = _process.StandardOutput.ReadToEnd();
            string error = _process.StandardError.ReadToEnd();

            await _process.WaitForExitAsync();

            return new ShellExecuteResult(string.IsNullOrEmpty(error), output, error);
        }

        public void Dispose()
        {
            _process.Dispose();
        }
    }
}