using DockerCliWrapper.Docker.Constants;
using DockerCliWrapper.Infrastructure;
using System.Threading.Tasks;

namespace DockerCliWrapper.Extensions
{
    public static class ShellExecutorExtensions
    {
        public static async Task<Result> ExecuteWithResult(this IShellExecutor shellExecutor, string arguments)
        {
            var shellResult = await shellExecutor.Execute(Commands.Docker, " " + arguments);

            return new Result(shellResult.IsSuccessFull, shellResult.Output, shellResult.Error);
        }
    }
}