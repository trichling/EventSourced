using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EventSourced.Simple.Framework
{

    public class ProjectionBase : IProjection
    {
        public ProjectionBase(string name, string stream)
        {
            Name = name;
            Stream = stream;
        }
        public string Name { get; init; }

        public string Stream { get; init; }

        public async Task Handle(dynamic @event)
        {
            await ((dynamic)this).Apply((dynamic)@event);
        }

        public Task Apply(object @event)
        {
            return Task.CompletedTask;
        }

    }

}