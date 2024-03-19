using Godot;
using im.Common.Godot;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CleanupFeature;

public class CleanupNode3DSystem : IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<Node3DLink> _node3DLinkPool;

    public void Run(IEcsSystems systems)
    {
        var filter = _world.Filter<Destroy>().Inc<Node3DLink>().End();

        foreach (var entity in filter)
        {
            ref var node = ref _node3DLinkPool.Get(entity);

            if (GodotObject.IsInstanceValid(node.Value))
            {
                node.Value.QueueFree();
            }
        }
    }
}