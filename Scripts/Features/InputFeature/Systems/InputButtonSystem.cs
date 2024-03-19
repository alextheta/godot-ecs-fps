using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.InputFeature;

public class InputButtonSystem : IEcsInitSystem, IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<InputButton> _inputButtonPool;
    [Inject] private EcsPool<InputButtonPressedEvent> _inputButtonPressedPool;
    [Inject] private EcsPool<InputButtonReleasedEvent> _inputButtonReleasedPool;

    public void Init(IEcsSystems systems)
    {
        var filter = _world.Filter<Input>().End();
        foreach (var entity in filter)
        {
            ref var inputButton = ref _inputButtonPool.Add(entity);

            inputButton.States = new();
            for (var i = 0; i < InputBinding.Actions.Length; i++)
            {
                inputButton.States.Add((InputAction)i, ButtonState.None);
            }
        }
    }

    public void Run(IEcsSystems systems)
    {
        var filter = _world.Filter<Input>().Inc<InputButton>().End();
        foreach (var entity in filter)
        {
            ref var inputButton = ref _inputButtonPool.Get(entity);

            foreach (var (action, _) in inputButton.States)
            {
                var actionName = InputBinding.Actions[(int)action];

                if (Godot.Input.Singleton.IsActionPressed(actionName))
                {
                    inputButton.States[action] = ButtonState.Pressed;
                }
                else if (Godot.Input.Singleton.IsActionJustReleased(actionName))
                {
                    inputButton.States[action] = ButtonState.Released;
                }
                else
                {
                    inputButton.States[action] = ButtonState.None;
                }

                if (Godot.Input.Singleton.IsActionJustPressed(actionName))
                {
                    _inputButtonPressedPool.Add(_world.NewEntity()).Action = action;
                }

                if (Godot.Input.Singleton.IsActionJustReleased(actionName))
                {
                    _inputButtonReleasedPool.Add(_world.NewEntity()).Action = action;
                }
            }
        }
    }
}