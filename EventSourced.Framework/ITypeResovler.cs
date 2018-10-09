using System;

namespace EventSourced.Framework
{
    public interface ITypeResovler
    {

        Type ResolveFrom(string eventTypeName);
    }
}