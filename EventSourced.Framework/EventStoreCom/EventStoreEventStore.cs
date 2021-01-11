// using System;
// using System.Collections.Generic;
// using System.Text;
// using System.Threading.Tasks;
// using EventSourced.Framework.Abstractions;
// using EventStore.Client;
// using Newtonsoft.Json;

// namespace EventSourced.Framework.EventStoreCom
// {
//     public class EventStoreEventStore : IEventStore
//     {
//         private readonly EventStoreClient eventStore;

//         public EventStoreEventStore(EventStoreClient eventStore)
//         {
//             this.eventStore = client;
//         }

//         public IDisposable CatchUpSubscription(long lastPosition, EventHandlerCallback eventHandler, Action hasCaughtUp)
//         {
//             throw new NotImplementedException();
//         }

//         public Task<IEnumerable<dynamic>> GetHistory(string persistenceId)
//         {
//             throw new NotImplementedException();
//         }

//         public async Task<long> Persist(string persistenceId, object @event)
//         {
//             var eventJsonUtf8 = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));
//             var message = new EventData(Uuid.NewUuid(), @event.GetType().Name, eventJsonUtf8);

//             await eventStore.AppendToStreamAsync(persistenceId, StreamState.Any, new [] { message });
//         }

//         public long StoreVersion()
//         {
//             return eventStore.
//         }
//     }
// }