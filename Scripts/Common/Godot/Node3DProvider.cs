using System;
using Godot;
using Leopotam.EcsLite;

namespace im.Common.Godot;

public abstract partial class Node3DProvider<T> : Node3D where T : struct
{
    public EcsWorld World { get; private set; }
    public EcsPool<T> Pool { get; private set; }
    public EcsPackedEntity PackedEntity { get; private set; }

    public ref T Data
    {
        get
        {
            if (PackedEntity.Unpack(World, out var entity))
            {
                return ref Pool.Get(entity);
            }

            throw new Exception($"Failed to unpack entity {entity} in provider for {typeof(T).Name}");
        }
    }

    public sealed override void _Ready()
    {
        World = GetNode<EcsWorldProvider>(EcsWorldProvider.NodePath).EcsWorld;

        var entity = World.NewEntity();
        World.GetPool<Node3DLink>().Add(entity) = new Node3DLink { Value = this };

        Pool = World.GetPool<T>();
        Pool.Add(entity);

        PackedEntity = World.PackEntity(entity);

        Init();
    }

    protected abstract void Init();
}