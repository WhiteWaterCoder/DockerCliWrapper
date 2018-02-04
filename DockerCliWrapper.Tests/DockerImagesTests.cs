using DockerCliWrapper.Docker.Images;
using DockerCliWrapper.Infrastructure;
using FluentAssertions;
using Moq;
using System.Text;
using Xunit;

namespace DockerCliWrapper.Tests
{
    public class GivenADockerImagesInstance
    {
        [Fact]
        public void WhenRequestingAllImages_ThenAllAreReturned()
        {
            var shellExecutor = new Mock<IShellExecutor>();

            var output = new StringBuilder();
            output.AppendLine("REPOSITORY                          TAG                   IMAGE ID            CREATED             SIZE");
            output.AppendLine("microsoft/aspnetcore-build          2.0-nanoserver-1709   f998ed08bddd        3 weeks ago         1.97GB");
            output.AppendLine("microsoft/aspnetcore                2.0-nanoserver-1709   db1a0a41a540        3 weeks ago         447MB");
            output.AppendLine("hello-world                         latest                ecea3d792cd1        3 weeks ago         303MB");

            shellExecutor
                .Setup(e => e.Execute("docker", " images -a"))
                .Returns(new ShellExecuteResult(true, output.ToString(), ""));

            var result = new DockerImages(shellExecutor.Object).ShowAll().Execute();

            result.Count.Should().Be(3);
            result[0].ImageId.Should().Be("f998ed08bddd");
            result[0].Repository.Should().Be("microsoft/aspnetcore-build");
            result[0].Tag.Should().Be("2.0-nanoserver-1709");
            result[0].CreatedSince.Should().Be("3 weeks ago");
            result[0].Size.Should().Be("1.97GB");
        }
    }
}