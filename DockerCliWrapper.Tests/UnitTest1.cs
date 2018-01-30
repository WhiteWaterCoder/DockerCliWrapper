using DockerCliWrapper.Docker;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DockerCliWrapper.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var result = new DockerImages().DoNotTruncate()
                                           .ShowAll()
                                           .Execute();
        }
    }
}