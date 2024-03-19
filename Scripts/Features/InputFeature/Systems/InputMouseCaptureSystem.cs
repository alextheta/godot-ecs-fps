using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.InputFeature;

public class InputMouseCaptureSystem : IEcsInitSystem, IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<InputMouseCaptureChanged> _inputMouseCaptureChangedEventPool;

    public void Init(IEcsSystems systems)
    {
        _inputMouseCaptureChangedEventPool.Add(_world.NewEntity()).Mode = Godot.Input.MouseModeEnum.Captured;
    }

    public void Run(IEcsSystems systems)
    {
        var filter = _world.Filter<InputMouseCaptureChanged>().End();
        foreach (var entity in filter)
        {
            ref var inputMouseCaptureChangedEvent = ref _inputMouseCaptureChangedEventPool.Get(entity);
            Godot.Input.MouseMode = inputMouseCaptureChangedEvent.Mode;
            _inputMouseCaptureChangedEventPool.Del(entity);
        }
    }
}