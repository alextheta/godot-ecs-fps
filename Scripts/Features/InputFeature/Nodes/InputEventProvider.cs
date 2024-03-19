using Godot;
using im.Common.Godot;
using Leopotam.EcsLite;

namespace im.Features.InputFeature.Nodes;

public partial class InputEventProvider : Node
{
    private EcsWorld _world;
    private EcsPool<InputMouseEvent> _inputMouseEventPool;

    public override void _Ready()
    {
        _world = GetNode<EcsWorldProvider>(EcsWorldProvider.NodePath).EcsWorld;
        _inputMouseEventPool = _world.GetPool<InputMouseEvent>();
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is not InputEventMouseMotion mouseInput)
        {
            return;
        }

        _inputMouseEventPool.Add(_world.NewEntity()) = new InputMouseEvent
        {
            Position = mouseInput.Position,
            Delta = mouseInput.Relative
        };
    }
}