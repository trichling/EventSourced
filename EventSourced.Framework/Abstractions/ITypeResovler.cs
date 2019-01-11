using System;

namespace EventSourced.Framework.Abstracions
{
    public interface ITypeResovler
    {

        Type ResolveFrom(string eventTypeName);
    }
}