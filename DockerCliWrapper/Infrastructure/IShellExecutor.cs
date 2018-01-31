using System;

namespace DockerCliWrapper.Infrastructure
{
    public interface IShellExecutor : IDisposable
    {
        ShellExecuteResult Execute(string command, string arguments);
    }
}