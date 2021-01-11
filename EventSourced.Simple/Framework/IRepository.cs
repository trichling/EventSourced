using System;
using System.Threading.Tasks;

namespace EventSourced.Simple.Framework
{

    public interface IRepository<T> where T : EventSourcedBase
    {
        Task<T> Get(Func<T> factory);

        Task Save(T aggregate);
    }

}