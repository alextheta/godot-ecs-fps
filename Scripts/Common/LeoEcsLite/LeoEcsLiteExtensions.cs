using System.Runtime.CompilerServices;
using Leopotam.EcsLite;

namespace im.Common.LeoEcsLite;

public static class LeoEcsLiteExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Set<T>(this EcsPool<T> pool, int entity) where T : struct
    {
        if (pool.Has(entity))
        {
            return ref pool.Get(entity);
        }

        return ref pool.Add(entity);
    }
}