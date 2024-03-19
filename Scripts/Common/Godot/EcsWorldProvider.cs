using Godot;
using Leopotam.EcsLite;

namespace im.Common.Godot;

public partial class EcsWorldProvider : Node
{
    public static NodePath NodePath { get; private set; }

    public EcsWorld EcsWorld { get; } = new();

    public override void _EnterTree()
    {
        NodePath = GetPath();
    }

    public override void _ExitTree()
    {
        EcsWorld.Destroy();
    }
}