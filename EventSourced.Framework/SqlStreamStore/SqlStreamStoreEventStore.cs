using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventSourced.Framework.Abstracions;
using Newtonsoft.Json;
using SqlStreamStore;
using SqlStreamStore.Streams;

namespace EventSourced.Framework.SqlStreamStore
{

    public class SqlStreamStoreEventStore : IEventStore
    {
        private readonly IStreamStore streamStore;
        private readonly ITypeResovler typeResovler;

        public SqlStreamStoreEventStore(IStreamStore streamStore, ITypeResovler typeResovler)
        {
            this.streamStore = streamStore;
            this.typeResovler = typeResovler;
            this.AllPersistenceIdsProjection = new AllPersistenceIdsSqlStreamStoreProjection(streamStore);
        }

        public IAllPersistenceIdsProjection AllPersistenceIdsProjection { get; }
        public async Task<IEnumerable<dynamic>> GetHistory(string persistenceId)
        {
            var history = new List<dynamic>();
            var streamId = new StreamId(persistenceId);
            var readStreamPage = await streamStore.ReadStreamForwards(streamId, StreamVersion.Start, int.MaxValue);

            foreach (var message in readStreamPage.Messages)
            {
                var eventJson = await message.GetJsonData();
                var eventType = typeResovler.ResolveFrom(message.Type);
                var @event = JsonConvert.DeserializeObject(eventJson, eventType);
                history.Add(@event);
            }

            return history;
        }

        public async Task<bool> Persist(string persistenceId, object @event)
        {
            var streamId = new StreamId(persistenceId);
            var message = new NewStreamMessage(Guid.NewGuid(), @event.GetType().Name, JsonConvert.SerializeObject(@event));
            var appendResult = await streamStore.AppendToStream(streamId, ExpectedVersion.Any, message);

            return true;
        }
    }
}