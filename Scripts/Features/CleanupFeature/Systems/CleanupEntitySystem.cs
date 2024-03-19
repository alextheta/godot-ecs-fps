using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CleanupFeature;

public class CleanupEntitySystem : IEcsRunSystem
{
    [Inject] private EcsWorld _world;

    public void Run(IEcsSystems systems)
    {
        var destroyFilter = _world.Filter<Destroy>().End();

        foreach (var entity in destroyFilter)
        {
            _world.DelEntity(entity);
        }
    }
}