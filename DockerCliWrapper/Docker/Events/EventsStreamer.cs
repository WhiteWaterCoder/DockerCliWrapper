using DockerCliWrapper.Docker.Container;
using DockerCliWrapper.Docker.Image;
using DockerCliWrapper.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace DockerCliWrapper.Docker.Events
{
    public class EventsStreamer
    {
        private readonly IObservable<string> _mainStream;

        public EventsStreamer()
            : this(new ShellExecutor())
        {
        }

        public EventsStreamer(IShellExecutor shellExecutor)
        {
            _mainStream = shellExecutor.ObserveStandardOutput("docker", " events --format \"{{json .}}\"")
                                       .SubscribeOn(NewThreadScheduler.Default)
                                       .Where(e => { return e.StartsWith("{ \"status\":"); });
                                       //.Select(json => { return JsonConvert.DeserializeObject<ContainerEvent>(json); });
        }

        public IObservable<ContainerEvent> GetContainerEventsObservable()
        {
            return _mainStream.Select(json => { return JsonConvert.DeserializeObject<ContainerEvent>(json); })
                              .Where(e => { return string.Equals(e.Type, "container"); });
        }

        public IObservable<ImageEvent> GetImageEventsObservable()
        {
            return _mainStream.Select(json => { return JsonConvert.DeserializeObject<ImageEvent>(json); })
                              .Where(e => { return string.Equals(e.Type, "image"); });
        }
    }
}