using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SqlStreamStore;
using SqlStreamStore.Streams;

namespace Lab.SqlStreamStoreDemo.Framework
{
    public class PersistEventHandler : IRequestHandler<PersistEvent, bool>
    {
        private readonly IStreamStore streamStore;

        public PersistEventHandler(IStreamStore streamStore)
        {
            this.streamStore = streamStore;
        }

        public async Task<bool> Handle(PersistEvent request, CancellationToken cancellationToken)
        {
            var streamId = new StreamId(request.PersistenceId);
            var message = new NewStreamMessage(Guid.NewGuid(), request.Event.GetType().Name, JsonConvert.SerializeObject(request.Event));
            var appendResult = await this.streamStore.AppendToStream(streamId, ExpectedVersion.Any, message);

            return true;
        }
    }
}
