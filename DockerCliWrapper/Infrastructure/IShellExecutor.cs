using System;

namespace DockerCliWrapper.Infrastructure
{
    public interface IShellExecutor : IDisposable
    {
        string Execute(string command, string arguments);
    }
}