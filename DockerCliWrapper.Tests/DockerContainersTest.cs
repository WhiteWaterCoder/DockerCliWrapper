using DockerCliWrapper.Docker.Constants;
using DockerCliWrapper.Docker.Container;
using DockerCliWrapper.Docker.Events;
using DockerCliWrapper.Infrastructure;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DockerCliWrapper.Tests
{
    public class GivenADockerContainersInstance
    {
        [Fact]
        public async Task WhenRequestingWithDefaultOptions_ThenResultsAreReturned()
        {
            var shellExecutor = new Mock<IShellExecutor>();

            var output = new StringBuilder();
            output.AppendLine("CONTAINER ID        IMAGE               COMMAND               CREATED             STATUS              PORTS                             NAMES");
            output.AppendLine("9860eb8aa0b3        service1            \"dotnet Service1.…\"   2 weeks ago         Up 27 hours         8002/tcp, 0.0.0.0:31206->80/tcp   test.service1");
            output.AppendLine("00f692571938        service2            \"dotnet Service2.…\"   2 weeks ago         Up 27 hours         8001/tcp, 0.0.0.0:6113->80/tcp    test.service2");

            shellExecutor
                .Setup(e => e.Execute(Commands.Docker, " container ls"))
                .ReturnsAsync(new ShellExecuteResult(true, output.ToString(), ""));

            var results = await new DockerContainers(shellExecutor.Object).SearchAsync();

            results.Count.Should().Be(2);
            results[0].Id.Should().Be("9860eb8aa0b3");
            results[0].Image.Should().Be("service1");
            results[0].Command.Should().Be("\"dotnet Service1.…\"");
            results[0].Created.Should().Be("2 weeks ago");
            results[0].Status.Should().Be("Up 27 hours");
            results[0].Ports.Should().Be("8002/tcp, 0.0.0.0:31206->80/tcp");
            results[0].Names.Should().Be("test.service1");
            results[0].Size.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task WhenRequestingSize_ThenResultsAreReturned()
        {
            var shellExecutor = new Mock<IShellExecutor>();

            var output = new StringBuilder();
            output.AppendLine("CONTAINER ID        IMAGE               COMMAND               CREATED             STATUS              PORTS                             NAMES          SIZE");
            output.AppendLine("9860eb8aa0b3        service1            \"dotnet Service1.…\"   2 weeks ago         Up 27 hours         8002/tcp, 0.0.0.0:31206->80/tcp   test.service1  10B");
            output.AppendLine("00f692571938        service2            \"dotnet Service2.…\"   2 weeks ago         Up 27 hours         8001/tcp, 0.0.0.0:6113->80/tcp    test.service2  20B");

            shellExecutor
                .Setup(e => e.Execute(Commands.Docker, " container ls -s"))
                .ReturnsAsync(new ShellExecuteResult(true, output.ToString(), ""));

            var results = await new DockerContainers(shellExecutor.Object).ShowSizes(true).SearchAsync();

            results.Count.Should().Be(2);
            results[0].Id.Should().Be("9860eb8aa0b3");
            results[0].Image.Should().Be("service1");
            results[0].Command.Should().Be("\"dotnet Service1.…\"");
            results[0].Created.Should().Be("2 weeks ago");
            results[0].Status.Should().Be("Up 27 hours");
            results[0].Ports.Should().Be("8002/tcp, 0.0.0.0:31206->80/tcp");
            results[0].Names.Should().Be("test.service1");
            results[0].Size.Should().Be("10B");
        }

        [Fact]
        public void WhenObservingContainerEvents_ThenEventsStartStreamingInForContainers()
        {
            var resetEvent = new ManualResetEventSlim();

            Task.Factory.StartNew(() => 
            {
                var containerEvents = new List<ContainerEvent>();

                var testScheduler = new TestScheduler();
                var shellExecutor = new Mock<IShellExecutor>();

                var outputStream = new Subject<string>();

                shellExecutor
                    .Setup(e => e.ObserveStandardOutput(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(outputStream);

                // start
                testScheduler.Schedule(TimeSpan.FromTicks(1), () => outputStream.OnNext("{ \"Type\":\"network\",\"Action\":\"connect\",\"Actor\":{ \"ID\":\"b11fbcc4b8d3510a3035522ddf9e2f8408a4dcfb72a3bf64a7145388fefc1019\",\"Attributes\":{ \"container\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"name\":\"nat\",\"type\":\"nat\"} },\"scope\":\"local\",\"time\":1522704104,\"timeNano\":1522704104089924100}"));
                testScheduler.Schedule(TimeSpan.FromTicks(2), () => outputStream.OnNext("{ \"status\":\"start\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"start\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\"} },\"scope\":\"local\",\"time\":1522704105,\"timeNano\":1522704105577256600}"));
                // stop
                testScheduler.Schedule(TimeSpan.FromTicks(3), () => outputStream.OnNext("{ \"status\":\"kill\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"kill\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\",\"signal\":\"15\"} },\"scope\":\"local\",\"time\":1522704122,\"timeNano\":1522704122497254900}"));
                testScheduler.Schedule(TimeSpan.FromTicks(4), () => outputStream.OnNext("{ \"status\":\"die\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"die\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"exitCode\":\"3221225786\",\"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\"} },\"scope\":\"local\",\"time\":1522704122,\"timeNano\":1522704122810754200}"));
                testScheduler.Schedule(TimeSpan.FromTicks(5), () => outputStream.OnNext("{ \"Type\":\"network\",\"Action\":\"disconnect\",\"Actor\":{ \"ID\":\"b11fbcc4b8d3510a3035522ddf9e2f8408a4dcfb72a3bf64a7145388fefc1019\",\"Attributes\":{ \"container\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"name\":\"nat\",\"type\":\"nat\"} },\"scope\":\"local\",\"time\":1522704122,\"timeNano\":1522704122852753400}"));
                testScheduler.Schedule(TimeSpan.FromTicks(6), () => outputStream.OnNext("{ \"status\":\"stop\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"stop\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\"} },\"scope\":\"local\",\"time\":1522704122,\"timeNano\":1522704122862754200}"));
                // start
                testScheduler.Schedule(TimeSpan.FromTicks(7), () => outputStream.OnNext("{ \"Type\":\"network\",\"Action\":\"connect\",\"Actor\":{ \"ID\":\"b11fbcc4b8d3510a3035522ddf9e2f8408a4dcfb72a3bf64a7145388fefc1019\",\"Attributes\":{ \"container\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"name\":\"nat\",\"type\":\"nat\"} },\"scope\":\"local\",\"time\":1522704147,\"timeNano\":1522704147964991000}"));
                testScheduler.Schedule(TimeSpan.FromTicks(8), () => outputStream.OnNext("{ \"status\":\"start\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"start\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\"} },\"scope\":\"local\",\"time\":1522704149,\"timeNano\":1522704149430387400}"));
                // kill
                testScheduler.Schedule(TimeSpan.FromTicks(9), () => outputStream.OnNext("{ \"status\":\"kill\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"kill\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\",\"signal\":\"9\"} },\"scope\":\"local\",\"time\":1522704166,\"timeNano\":1522704166982836400}"));
                testScheduler.Schedule(TimeSpan.FromTicks(10), () => outputStream.OnNext("{ \"status\":\"die\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"die\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"exitCode\":\"4294967295\",\"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\"} },\"scope\":\"local\",\"time\":1522704166,\"timeNano\":1522704166983837400}"));
                testScheduler.Schedule(TimeSpan.FromTicks(11), () => outputStream.OnNext("{ \"Type\":\"network\",\"Action\":\"disconnect\",\"Actor\":{ \"ID\":\"b11fbcc4b8d3510a3035522ddf9e2f8408a4dcfb72a3bf64a7145388fefc1019\",\"Attributes\":{ \"container\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"name\":\"nat\",\"type\":\"nat\"} },\"scope\":\"local\",\"time\":1522704167,\"timeNano\":1522704167017841200}"));
                // start
                testScheduler.Schedule(TimeSpan.FromTicks(12), () => outputStream.OnNext("{ \"Type\":\"network\",\"Action\":\"connect\",\"Actor\":{ \"ID\":\"b11fbcc4b8d3510a3035522ddf9e2f8408a4dcfb72a3bf64a7145388fefc1019\",\"Attributes\":{ \"container\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"name\":\"nat\",\"type\":\"nat\"} },\"scope\":\"local\",\"time\":1522704186,\"timeNano\":1522704186955939900}"));
                testScheduler.Schedule(TimeSpan.FromTicks(13), () => outputStream.OnNext("{ \"status\":\"start\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"start\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\"} },\"scope\":\"local\",\"time\":1522704188,\"timeNano\":1522704188388571200}"));
                //pause
                testScheduler.Schedule(TimeSpan.FromTicks(14), () => outputStream.OnNext("{ \"status\":\"pause\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"pause\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\"} },\"scope\":\"local\",\"time\":1522704207,\"timeNano\":1522704207487173200}"));
                // unpause
                testScheduler.Schedule(TimeSpan.FromTicks(15), () => outputStream.OnNext("{ \"status\":\"unpause\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"unpause\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\"} },\"scope\":\"local\",\"time\":1522704224,\"timeNano\":1522704224102358100}"));

                var subscription = new EventsStreamer(shellExecutor.Object).GetContainerEventsObservable()
                                                 .ObserveOn(Scheduler.Default)
                                                 .Subscribe(x => { containerEvents.Add(x); });

                testScheduler.AdvanceBy(1);
                containerEvents.Count.Should().Be(0);
                testScheduler.AdvanceBy(1);
                Thread.Sleep(100);
                containerEvents.Count.Should().Be(1);
                containerEvents.ElementAt(0).ShortId.Should().Be("c82386cd35b3");
                containerEvents.ElementAt(0).EventStatus.Should().Be(ContainerEventStatus.Start);
                testScheduler.AdvanceBy(1);
                Thread.Sleep(100);
                containerEvents.Count.Should().Be(2);
                containerEvents.ElementAt(1).ShortId.Should().Be("c82386cd35b3");
                containerEvents.ElementAt(1).EventStatus.Should().Be(ContainerEventStatus.Stop);
                testScheduler.AdvanceBy(1);
                Thread.Sleep(100);
                containerEvents.Count.Should().Be(3);
                containerEvents.ElementAt(2).ShortId.Should().Be("c82386cd35b3");
                containerEvents.ElementAt(2).EventStatus.Should().Be(ContainerEventStatus.Start);
                testScheduler.AdvanceBy(1);
                Thread.Sleep(100);
                containerEvents.Count.Should().Be(5);
                containerEvents.ElementAt(4).ShortId.Should().Be("c82386cd35b3");
                containerEvents.ElementAt(4).EventStatus.Should().Be(ContainerEventStatus.Die);
                testScheduler.AdvanceBy(1);
                Thread.Sleep(100);
                containerEvents.Count.Should().Be(6);
                containerEvents.ElementAt(5).ShortId.Should().Be("c82386cd35b3");
                containerEvents.ElementAt(5).EventStatus.Should().Be(ContainerEventStatus.Start);
                testScheduler.AdvanceBy(1);
                Thread.Sleep(100);
                containerEvents.Count.Should().Be(7);
                containerEvents.ElementAt(6).ShortId.Should().Be("c82386cd35b3");
                containerEvents.ElementAt(6).EventStatus.Should().Be(ContainerEventStatus.Pause);
                testScheduler.AdvanceBy(1);
                Thread.Sleep(100);
                containerEvents.Count.Should().Be(8);
                containerEvents.ElementAt(7).ShortId.Should().Be("c82386cd35b3");
                containerEvents.ElementAt(7).EventStatus.Should().Be(ContainerEventStatus.Unpause);

                resetEvent.Set();
            });

            while (!resetEvent.IsSet)
            {
                Thread.Sleep(500);
            }
        }
    }
}