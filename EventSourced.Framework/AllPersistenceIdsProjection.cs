using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading; 
using System.Threading.Tasks; 
using SqlStreamStore;
using SqlStreamStore.Streams;
using SqlStreamStore.Subscriptions; 

namespace EventSourced.Framework {

    public class AllPersistenceIdsProjection : IAllPersistenceIdsProjection 
    {
        private readonly IStreamStore streamStore; 
        private HashSet<StreamId> streamIds;
        private bool hasCaughtUp;
        private long lastPosition;

        public AllPersistenceIdsProjection(IStreamStore streamStore) 
        {
            this.streamIds = new HashSet<StreamId>();
            this.streamStore = streamStore; 

            this.streamStore.SubscribeToAll(null, NotifAllStreamMessageReceived, NotifAllSubscriptionDropped, NotifyHasCaughtUp);
        }

        public bool IsUpToDate => hasCaughtUp && lastPosition == streamStore.ReadHeadPosition().Result;

        public ReadOnlyCollection<StreamId> StreamIds => new ReadOnlyCollection<StreamId>(streamIds.ToList());

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