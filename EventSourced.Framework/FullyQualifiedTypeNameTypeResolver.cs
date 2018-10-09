
using System;

namespace EventSourced.Framework
{
    public class FullyQualifiedTypeNameTypeResolver : ITypeResovler
    {
        private readonly string typeNameTemplate;

        public FullyQualifiedTypeNameTypeResolver(string typeNameTemplate)
        {
            this.typeNameTemplate = typeNameTemplate;
        }

        public Type ResolveFrom(string eventName)
        {
            var eventTypeName = string.Format(typeNameTemplate, eventName);
            var eventType = Type.GetType(eventTypeName);
            return eventType;
        }
    }
}