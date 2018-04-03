using DockerCliWrapper.Extensions;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DockerCliWrapper.Infrastructure
{
    public class ShellExecutor : IShellExecutor
    {
        private static readonly ShellExecutor _instance = new ShellExecutor();

        private readonly Process _process;

        public ShellExecutor()
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

        public IObservable<string> ObserveStandardOutput(string command, string arguments)
        {
            var process = new Process
            {
                EnableRaisingEvents = true
            };

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.Start();

            var received = Observable
                .FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
                    handler => handler.Invoke,
                    h => process.OutputDataReceived += h,
                    h => process.OutputDataReceived -= h)
                .TakeUntil(Observable.FromEventPattern(
                    h => process.Exited += h,
                    h => process.Exited -= h))
                .Select(e => e.EventArgs.Data);

            process.BeginOutputReadLine();

            return received;
        }

        public void Dispose()
        {
            _process.Dispose();
        }
    }
}