using System;

namespace EventSourced.Framework.Abstractions
{
    public interface ITypeResovler
    {

        Type ResolveFrom(string eventTypeName);
    }
}