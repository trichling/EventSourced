using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading; 
using System.Threading.Tasks;
using EventSourced.Framework.Abstracions;
using SqlStreamStore;
using SqlStreamStore.Streams;
using SqlStreamStore.Subscriptions; 

namespace EventSourced.Framework .SqlStreamStore
{

    public class AllPersistenceIdsSqlStreamStoreProjection : IAllPersistenceIdsProjection 
    {
        private readonly IStreamStore streamStore; 
        private HashSet<StreamId> streamIds;
        private bool hasCaughtUp;
        private long lastPosition;

        public AllPersistenceIdsSqlStreamStoreProjection(IStreamStore streamStore) 
        {
            this.streamIds = new HashSet<StreamId>();
            this.streamStore = streamStore; 

            this.streamStore.SubscribeToAll(null, NotifAllStreamMessageReceived, NotifAllSubscriptionDropped, NotifyHasCaughtUp);
        }

        public bool IsUpToDate => hasCaughtUp && lastPosition == streamStore.ReadHeadPosition().Result;

        public ReadOnlyCollection<string> StreamIds => new ReadOnlyCollection<string>(streamIds.Select(s => s.Value).ToList());

        public async Task WaitUntilIsUpToDate()
        {
            while (!IsUpToDate)
                await Task.Delay(10);
        }

        private async Task NotifAllStreamMessageReceived(IAllStreamSubscription subscription, StreamMessage streamMessage, CancellationToken cancellationToken) 
        {
            streamIds.Add(new StreamId(streamMessage.StreamId));
            lastPosition = streamMessage.Position;
        }

        private void NotifyHasCaughtUp(bool hasCaughtUp) => this.hasCaughtUp = hasCaughtUp;

        private void NotifAllSubscriptionDropped(IAllStreamSubscription subscription, SubscriptionDroppedReason reason, Exception exception = null) 
        {
        }
      
    }

}