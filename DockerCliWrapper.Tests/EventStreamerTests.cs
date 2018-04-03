using DockerCliWrapper.Docker.Container;
using DockerCliWrapper.Docker.Events;
using DockerCliWrapper.Docker.Image;
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
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DockerCliWrapper.Tests
{
    public class EventStreamerTests
    {
        [Fact]
        public void WhenObservingImageEvents_ThenEventsStartStreamingInForImages()
        {
            var resetEvent = new ManualResetEventSlim();

            Task.Factory.StartNew(() =>
            {
                var imageEvents = new List<ImageEvent>();

                var testScheduler = new TestScheduler();
                var shellExecutor = new Mock<IShellExecutor>();

                var outputStream = new Subject<string>();

                shellExecutor
                    .Setup(e => e.ObserveStandardOutput(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(outputStream);

                testScheduler.Schedule(TimeSpan.FromTicks(1), () => outputStream.OnNext("{\"status\":\"tag\",\"id\":\"sha256:ecea3d792cd143caccdf16935cfa1e4d0ec566a6d9e64eac25dfe9087c806702\",\"Type\":\"image\",\"Action\":\"untag\",\"Actor\":{\"ID\":\"sha256:ecea3d792cd143caccdf16935cfa1e4d0ec566a6d9e64eac25dfe9087c806702\",\"Attributes\":{\"name\":\"sha256:ecea3d792cd143caccdf16935cfa1e4d0ec566a6d9e64eac25dfe9087c806702\"}},\"scope\":\"local\",\"time\":1522790236,\"timeNano\":1522790236084372200}"));
                testScheduler.Schedule(TimeSpan.FromTicks(2), () => outputStream.OnNext("{\"status\":\"untag\",\"id\":\"sha256:ecea3d792cd143caccdf16935cfa1e4d0ec566a6d9e64eac25dfe9087c806702\",\"Type\":\"image\",\"Action\":\"untag\",\"Actor\":{\"ID\":\"sha256:ecea3d792cd143caccdf16935cfa1e4d0ec566a6d9e64eac25dfe9087c806702\",\"Attributes\":{\"name\":\"sha256:ecea3d792cd143caccdf16935cfa1e4d0ec566a6d9e64eac25dfe9087c806702\"}},\"scope\":\"local\",\"time\":1522790236,\"timeNano\":1522790236084372200}"));
                testScheduler.Schedule(TimeSpan.FromTicks(3), () => outputStream.OnNext("{\"status\":\"delete\",\"id\":\"sha256:ecea3d792cd143caccdf16935cfa1e4d0ec566a6d9e64eac25dfe9087c806702\",\"Type\":\"image\",\"Action\":\"delete\",\"Actor\":{\"ID\":\"sha256:ecea3d792cd143caccdf16935cfa1e4d0ec566a6d9e64eac25dfe9087c806702\",\"Attributes\":{\"name\":\"sha256:ecea3d792cd143caccdf16935cfa1e4d0ec566a6d9e64eac25dfe9087c806702\"}},\"scope\":\"local\",\"time\":1522790236,\"timeNano\":1522790236135375000}"));
                testScheduler.Schedule(TimeSpan.FromTicks(4), () => outputStream.OnNext("{\"status\":\"pull\",\"id\":\"hello-world:latest\",\"Type\":\"image\",\"Action\":\"pull\",\"Actor\":{\"ID\":\"hello-world:latest\",\"Attributes\":{\"name\":\"hello-world\"}},\"scope\":\"local\",\"time\":1522790368,\"timeNano\":1522790368874018400}"));

                var subscription = new EventsStreamer(shellExecutor.Object).GetImageEventsObservable()
                                                 .ObserveOn(Scheduler.Default)
                                                 .Subscribe(x => { imageEvents.Add(x); });

                testScheduler.AdvanceBy(1);
                Thread.Sleep(100);
                imageEvents.Count.Should().Be(1);
                imageEvents.ElementAt(0).ShortId.Should().Be("ecea3d792cd1");
                imageEvents.ElementAt(0).EventStatus.Should().Be(ImageEventStatus.Tag);
                testScheduler.AdvanceBy(1);
                Thread.Sleep(100);
                imageEvents.Count.Should().Be(2);
                imageEvents.ElementAt(1).ShortId.Should().Be("ecea3d792cd1");
                imageEvents.ElementAt(1).EventStatus.Should().Be(ImageEventStatus.Untag);
                testScheduler.AdvanceBy(1);
                Thread.Sleep(100);
                imageEvents.Count.Should().Be(3);
                imageEvents.ElementAt(2).ShortId.Should().Be("ecea3d792cd1");
                imageEvents.ElementAt(2).EventStatus.Should().Be(ImageEventStatus.Delete);
                testScheduler.AdvanceBy(1);
                Thread.Sleep(100);
                imageEvents.Count.Should().Be(4);
                imageEvents.ElementAt(3).ShortId.Should().Be("ecea3d792cd1");
                imageEvents.ElementAt(3).EventStatus.Should().Be(ImageEventStatus.Pull);

                resetEvent.Set();
            });

            resetEvent.Wait();
            resetEvent.Dispose();
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
                testScheduler.Schedule(TimeSpan.FromTicks(1), () => outputStream.OnNext("{\"Type\":\"network\",\"Action\":\"connect\",\"Actor\":{ \"ID\":\"b11fbcc4b8d3510a3035522ddf9e2f8408a4dcfb72a3bf64a7145388fefc1019\",\"Attributes\":{ \"container\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"name\":\"nat\",\"type\":\"nat\"} },\"scope\":\"local\",\"time\":1522704104,\"timeNano\":1522704104089924100}"));
                testScheduler.Schedule(TimeSpan.FromTicks(2), () => outputStream.OnNext("{\"status\":\"start\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"start\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\"} },\"scope\":\"local\",\"time\":1522704105,\"timeNano\":1522704105577256600}"));
                // stop
                testScheduler.Schedule(TimeSpan.FromTicks(3), () => outputStream.OnNext("{\"status\":\"kill\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"kill\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\",\"signal\":\"15\"} },\"scope\":\"local\",\"time\":1522704122,\"timeNano\":1522704122497254900}"));
                testScheduler.Schedule(TimeSpan.FromTicks(4), () => outputStream.OnNext("{\"status\":\"die\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"die\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"exitCode\":\"3221225786\",\"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\"} },\"scope\":\"local\",\"time\":1522704122,\"timeNano\":1522704122810754200}"));
                testScheduler.Schedule(TimeSpan.FromTicks(5), () => outputStream.OnNext("{\"Type\":\"network\",\"Action\":\"disconnect\",\"Actor\":{ \"ID\":\"b11fbcc4b8d3510a3035522ddf9e2f8408a4dcfb72a3bf64a7145388fefc1019\",\"Attributes\":{ \"container\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"name\":\"nat\",\"type\":\"nat\"} },\"scope\":\"local\",\"time\":1522704122,\"timeNano\":1522704122852753400}"));
                testScheduler.Schedule(TimeSpan.FromTicks(6), () => outputStream.OnNext("{\"status\":\"stop\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"stop\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\"} },\"scope\":\"local\",\"time\":1522704122,\"timeNano\":1522704122862754200}"));
                // start
                testScheduler.Schedule(TimeSpan.FromTicks(7), () => outputStream.OnNext("{\"Type\":\"network\",\"Action\":\"connect\",\"Actor\":{ \"ID\":\"b11fbcc4b8d3510a3035522ddf9e2f8408a4dcfb72a3bf64a7145388fefc1019\",\"Attributes\":{ \"container\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"name\":\"nat\",\"type\":\"nat\"} },\"scope\":\"local\",\"time\":1522704147,\"timeNano\":1522704147964991000}"));
                testScheduler.Schedule(TimeSpan.FromTicks(8), () => outputStream.OnNext("{\"status\":\"start\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"start\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\"} },\"scope\":\"local\",\"time\":1522704149,\"timeNano\":1522704149430387400}"));
                // kill
                testScheduler.Schedule(TimeSpan.FromTicks(9), () => outputStream.OnNext("{\"status\":\"kill\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"kill\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\",\"signal\":\"9\"} },\"scope\":\"local\",\"time\":1522704166,\"timeNano\":1522704166982836400}"));
                testScheduler.Schedule(TimeSpan.FromTicks(10), () => outputStream.OnNext("{\"status\":\"die\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"die\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"exitCode\":\"4294967295\",\"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\"} },\"scope\":\"local\",\"time\":1522704166,\"timeNano\":1522704166983837400}"));
                testScheduler.Schedule(TimeSpan.FromTicks(11), () => outputStream.OnNext("{\"Type\":\"network\",\"Action\":\"disconnect\",\"Actor\":{ \"ID\":\"b11fbcc4b8d3510a3035522ddf9e2f8408a4dcfb72a3bf64a7145388fefc1019\",\"Attributes\":{ \"container\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"name\":\"nat\",\"type\":\"nat\"} },\"scope\":\"local\",\"time\":1522704167,\"timeNano\":1522704167017841200}"));
                // start
                testScheduler.Schedule(TimeSpan.FromTicks(12), () => outputStream.OnNext("{\"Type\":\"network\",\"Action\":\"connect\",\"Actor\":{ \"ID\":\"b11fbcc4b8d3510a3035522ddf9e2f8408a4dcfb72a3bf64a7145388fefc1019\",\"Attributes\":{ \"container\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"name\":\"nat\",\"type\":\"nat\"} },\"scope\":\"local\",\"time\":1522704186,\"timeNano\":1522704186955939900}"));
                testScheduler.Schedule(TimeSpan.FromTicks(13), () => outputStream.OnNext("{\"status\":\"start\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"start\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\"} },\"scope\":\"local\",\"time\":1522704188,\"timeNano\":1522704188388571200}"));
                //pause
                testScheduler.Schedule(TimeSpan.FromTicks(14), () => outputStream.OnNext("{\"status\":\"pause\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"pause\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\"} },\"scope\":\"local\",\"time\":1522704207,\"timeNano\":1522704207487173200}"));
                // unpause
                testScheduler.Schedule(TimeSpan.FromTicks(15), () => outputStream.OnNext("{\"status\":\"unpause\",\"id\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"from\":\"dc360deba4fc\",\"Type\":\"container\",\"Action\":\"unpause\",\"Actor\":{ \"ID\":\"c82386cd35b33d95559b6f4bb217da1190e2754f42459b4cc9440f0fb69f5a50\",\"Attributes\":{ \"image\":\"dc360deba4fc\",\"name\":\"nifty_liskov\"} },\"scope\":\"local\",\"time\":1522704224,\"timeNano\":1522704224102358100}"));

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
                containerEvents.ElementAt(1).EventStatus.Should().Be(ContainerEventStatus.Kill);
                testScheduler.AdvanceBy(1);
                Thread.Sleep(100);
                containerEvents.Count.Should().Be(3);
                containerEvents.ElementAt(2).ShortId.Should().Be("c82386cd35b3");
                containerEvents.ElementAt(2).EventStatus.Should().Be(ContainerEventStatus.Die);
                testScheduler.AdvanceBy(2);
                Thread.Sleep(100);
                containerEvents.Count.Should().Be(4);
                containerEvents.ElementAt(3).ShortId.Should().Be("c82386cd35b3");
                containerEvents.ElementAt(3).EventStatus.Should().Be(ContainerEventStatus.Stop);
                testScheduler.AdvanceBy(2);
                Thread.Sleep(100);
                containerEvents.Count.Should().Be(5);
                containerEvents.ElementAt(4).ShortId.Should().Be("c82386cd35b3");
                containerEvents.ElementAt(4).EventStatus.Should().Be(ContainerEventStatus.Start);
                testScheduler.AdvanceBy(1);
                Thread.Sleep(100);
                containerEvents.Count.Should().Be(6);
                containerEvents.ElementAt(5).ShortId.Should().Be("c82386cd35b3");
                containerEvents.ElementAt(5).EventStatus.Should().Be(ContainerEventStatus.Kill);
                testScheduler.AdvanceBy(1);
                Thread.Sleep(100);
                containerEvents.Count.Should().Be(7);
                containerEvents.ElementAt(6).ShortId.Should().Be("c82386cd35b3");
                containerEvents.ElementAt(6).EventStatus.Should().Be(ContainerEventStatus.Die);
                testScheduler.AdvanceBy(3);
                Thread.Sleep(100);
                containerEvents.Count.Should().Be(8);
                containerEvents.ElementAt(7).ShortId.Should().Be("c82386cd35b3");
                containerEvents.ElementAt(7).EventStatus.Should().Be(ContainerEventStatus.Start);
                testScheduler.AdvanceBy(1);
                Thread.Sleep(100);
                containerEvents.Count.Should().Be(9);
                containerEvents.ElementAt(8).ShortId.Should().Be("c82386cd35b3");
                containerEvents.ElementAt(8).EventStatus.Should().Be(ContainerEventStatus.Pause);
                testScheduler.AdvanceBy(1);
                Thread.Sleep(100);
                containerEvents.Count.Should().Be(10);
                containerEvents.ElementAt(9).ShortId.Should().Be("c82386cd35b3");
                containerEvents.ElementAt(9).EventStatus.Should().Be(ContainerEventStatus.Unpause);

                resetEvent.Set();
            });

            resetEvent.Wait();
            resetEvent.Dispose();
        }
    }
}