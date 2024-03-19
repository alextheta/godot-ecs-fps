using Godot;
using im.Features.CleanupFeature;
using Leopotam.EcsLite;

namespace im.Common.Godot;

public abstract partial class Node3DRoot : Node3D
{
    public EcsWorld World { get; set; }
    public EcsPackedEntity PackedEntity { get; set; }

    public sealed override void _ExitTree()
    {
        if (World != null && PackedEntity.Unpack(World, out var entity))
        {
            World.GetPool<Destroy>().Add(entity);
        }
    }
}