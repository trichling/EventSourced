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

    public class AllPersistenceIdsInMemoryProjection : IAllPersistenceIdsProjection 
    {
        private readonly IEnumerable<string> persistenceIds;

        public AllPersistenceIdsInMemoryProjection(IEnumerable<string> persistenceIds) 
        {
            this.persistenceIds = persistenceIds;
        }

        public bool IsUpToDate => true;

        public ReadOnlyCollection<string> StreamIds => new ReadOnlyCollection<string>(persistenceIds.ToList());

        public Task WaitUntilIsUpToDate()
        {
            return Task.CompletedTask;
        }
      
    }

}