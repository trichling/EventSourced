using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using EventSourced.Framework.Abstractions;

namespace EventSourced.Framework
{

    public class AllPersistenceIdsReadModel : ReadModelBase
    {
        private HashSet<string> persistenceIds;

        public AllPersistenceIdsReadModel(IEventSourcingSystem system) : base(system)
        {
            this.persistenceIds = new HashSet<string>();
            this.StartCatchingUpFrom(0);
        }
        
        public ReadOnlyCollection<string> PersistenceIds => new ReadOnlyCollection<string>(persistenceIds.ToList());

        protected override void OnEvent(string persistenceId, long position, object @event)
        {
            lastPosition = position;
            persistenceIds.Add(persistenceId);
        }

    }
}