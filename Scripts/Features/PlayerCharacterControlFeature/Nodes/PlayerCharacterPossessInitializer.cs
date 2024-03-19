using Godot;
using im.Features.CharacterFeature;
using Leopotam.EcsLite;

namespace im.Features.PlayerCharacterControlFeature;

public partial class PlayerCharacterPossessInitializer : Node
{
    public override void _Ready()
    {
        var localRoot = GetOwner<CharacterRoot>();
        var world = localRoot.World;
        if (localRoot.PackedEntity.Unpack(world, out var entity))
        {
            world.GetPool<CharacterPossess>().Add(entity);
        }

        QueueFree();
    }
}