using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Lab.SqlStreamStoreDemo.Framework
{

    public class PublishedEventHandler : INotificationHandler<PublishedEvent>
    {
        private readonly EventHandlerRepository eventHandlerList;

        public PublishedEventHandler(EventHandlerRepository eventHandlerList)
        {
            this.eventHandlerList = eventHandlerList;
        }

        public Task Handle(PublishedEvent notification, CancellationToken cancellationToken)
        {
           eventHandlerList.Invoke(notification);
           return Task.CompletedTask;
        }

    }

}