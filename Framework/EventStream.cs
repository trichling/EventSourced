﻿using System;
using System.Collections.Generic;

namespace Lab.SqlStreamStoreDemo.Framework
{
    public class EventStream : IEventStream
    {
        private Dictionary<Type, List<dynamic>> handlerList = new Dictionary<Type, List<dynamic>>();

         public void Subscribe<T>(Action<T> handler)
        {
            if (!handlerList.ContainsKey(typeof(T)))
                handlerList.Add(typeof(T), new List<dynamic>());

            handlerList[typeof(T)].Add(handler);
        }

        public void Publish(object notification)
        {
            var eventType = notification.GetType();

            if (!handlerList.ContainsKey(eventType))
                return;

            var handlers = handlerList[notification.GetType()];
            foreach (var handler in handlers)
            {
                ((Delegate)handler).DynamicInvoke(notification);
            }

            return;
        }
    }
}