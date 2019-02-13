using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourced.Framework.Abstracions
{

    public class AllPresistenceIdsReadModel : ReadModelBase
    {
        private HashSet<string> persistenceIds;

        public AllPresistenceIdsReadModel(IEventSourcingSystem system) : base(system)
        {
            this.persistenceIds = new HashSet<string>();
            this.CatchUp();
        }
        
        public ReadOnlyCollection<string> PersistenceIds => new ReadOnlyCollection<string>(persistenceIds.ToList());

        protected override void OnEvent(string persistenceId, dynamic @event, long position)
        {
            persistenceIds.Add(persistenceId);
        }
    }
}