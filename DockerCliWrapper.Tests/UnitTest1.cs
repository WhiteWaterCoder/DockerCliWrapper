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

            //var result2 = new DockerImage("hello-world").Remove(out string s);

            var result3 = new DockerImage("hello-world").ShowHistory().DoNotTruncate().Execute();
        }
    }
}