using DockerCliWrapper.Docker.Container;
using DockerCliWrapper.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace DockerCliWrapper.Docker.Events
{
    public class EventsStreamer
    {
        private readonly IObservable<ContainerEvent> _mainStream;

        public EventsStreamer()
            : this(new ShellExecutor())
        {
        }

        public EventsStreamer(IShellExecutor shellExecutor)
        {
            _mainStream = shellExecutor.ObserveStandardOutput("docker", " events --format \"{{json .}}\"")
                                       .SubscribeOn(NewThreadScheduler.Default)
                                       .Where(e => { return e.StartsWith("{ \"status\":"); })
                                       .Select(json => { return JsonConvert.DeserializeObject<ContainerEvent>(json); });
        }

        public IObservable<ContainerEvent> GetContainerEventsObservable()
        {
            return _mainStream.Where(e => { return string.Equals(e.Type, "container"); });
        }
    }
}