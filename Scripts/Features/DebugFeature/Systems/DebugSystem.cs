using Godot;
using im.Features.InputFeature;
using Leopotam.EcsLite;
using Te.DI;
using Input = Godot.Input;

namespace im.Features.DebugFeature;

public class DebugSystem : IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<InputButtonPressedEvent> _inputButtonPressedEventPool;
    [Inject] private EcsPool<InputMouseCaptureChanged> _inputMouseCaptureChangedEventPool;

    [Inject] private SceneTree _sceneTree;

    public void Run(IEcsSystems systems)
    {
        var filter = _world.Filter<InputButtonPressedEvent>().End();

        foreach (var entity in filter)
        {
            ref var button = ref _inputButtonPressedEventPool.Get(entity);

            switch (button.Action)
            {
                case InputAction.LMB:
                    _inputMouseCaptureChangedEventPool.Add(_world.NewEntity()).Mode = Input.MouseModeEnum.Captured;
                    break;
                case InputAction.Escape:
                    if (Input.MouseMode != Input.MouseModeEnum.Captured)
                    {
                        _sceneTree.Quit();
                    }
                    else
                    {
                        _inputMouseCaptureChangedEventPool.Add(_world.NewEntity()).Mode = Input.MouseModeEnum.Visible;
                    }

                    break;
            }
        }
    }
}