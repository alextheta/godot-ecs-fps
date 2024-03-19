using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.InputFeature;

public class InputInitSystem : IEcsInitSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<Input> _inputPool;

    public void Init(IEcsSystems systems)
    {
        var inputEntity = _world.NewEntity();
        _inputPool.Add(inputEntity);
    }
}