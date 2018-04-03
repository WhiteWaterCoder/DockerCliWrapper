using System;
using System.Threading.Tasks;

namespace DockerCliWrapper.Infrastructure
{
    public interface IShellExecutor : IDisposable
    {
        Task<ShellExecuteResult> Execute(string command, string arguments);

        IObservable<string> ObserveStandardOutput(string command, string arguments);
    }
}