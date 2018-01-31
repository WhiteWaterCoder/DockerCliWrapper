using DockerCliWrapper.Docker.Image;
using DockerCliWrapper.Docker.Images;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DockerCliWrapper.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //var result = new DockerImages().DoNotTruncate()
            //                               .ShowAll()
            //                               .Execute();

            var result2 = new DockerImage().Remove("hello-world", out string s);
        }
    }
}