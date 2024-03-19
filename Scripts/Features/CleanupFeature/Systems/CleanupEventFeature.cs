using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CleanupFeature;

public class CleanupEventFeature<T> : IEcsInitSystem, IEcsRunSystem where T : struct
{
    [Inject] private EcsWorld _world;
    private EcsPool<T> _pool;

    public void Init(IEcsSystems systems)
    {
        _pool = _world.GetPool<T>();
    }

    public void Run(IEcsSystems systems)
    {
        var destroyFilter = _world.Filter<T>().End();

        foreach (var entity in destroyFilter)
        {
            _pool.Del(entity);
        }
    }
}