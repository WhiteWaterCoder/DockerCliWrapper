using System.Diagnostics;

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
            _process.StartInfo.RedirectStandardOutput = true;
        }

        public static IShellExecutor Instance => _instance;

        public string Execute(string command, string arguments)
        {
            _process.StartInfo.FileName = command;
            _process.StartInfo.Arguments = arguments;
            _process.Start();
            
            string output = _process.StandardOutput.ReadToEnd();

            _process.WaitForExit();

            return output;
        }

        public void Dispose()
        {
            _process.Dispose();
        }
    }
}