using Godot;
using im.Common.Godot;
using im.Features.CleanupFeature;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CharacterFeature;

public class CharacterSpawnSystem : IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<Character> _characterPool;
    [Inject] private EcsPool<CharacterSpawn> _characterSpawnPool;
    [Inject] private EcsPool<CharacterBodyLink> _characterBodyLinkPool;
    [Inject] private EcsPool<CharacterSettings> _characterSettingsPool;
    [Inject] private EcsPool<CharacterColliderLink> _characterColliderLinkPool;
    [Inject] private EcsPool<CharacterSmoothMovement> _characterSmoothMovementPool;
    [Inject] private EcsPool<CharacterMovementSpeed> _characterMovementSpeedPool;
    [Inject] private EcsPool<Node3DLink> _node3DLinkPool;
    [Inject] private EcsPool<Destroy> _destroyPool;

    public void Run(IEcsSystems systems)
    {
        var spawnFilter = _world.Filter<CharacterSpawn>().End();

        foreach (var entity in spawnFilter)
        {
            ref var spawn = ref _characterSpawnPool.Get(entity);
            var characterSettings = spawn.Settings;
            var characterRootNode = spawn.Prefab.Instantiate<CharacterRoot>();
            var characterBodyNode = characterRootNode.CharacterBody;
            var characterHeadNode = characterBodyNode.Head;
            var characterCollider = characterBodyNode.Collider;
            var characterColliderShape = characterCollider.Shape as CapsuleShape3D;

            var headHeight = characterSettings.BaseHeight * (1f - characterSettings.HeadOffsetPercent);
            characterHeadNode.Position = new Vector3(0, headHeight, 0);

            var characterEntity = _world.NewEntity();

            _characterPool.Add(characterEntity);
            _node3DLinkPool.Add(characterEntity) = new Node3DLink { Value = characterRootNode };
            _characterBodyLinkPool.Add(characterEntity) = new CharacterBodyLink { Value = characterBodyNode };
            _characterSmoothMovementPool.Add(characterEntity) = new CharacterSmoothMovement { Direction = Vector3.Zero };
            _characterMovementSpeedPool.Add(characterEntity) = new CharacterMovementSpeed { Value = characterSettings.BaseSpeed };
            _characterSettingsPool.Add(characterEntity) = new CharacterSettings { Value = characterSettings };
            _characterColliderLinkPool.Add(characterEntity) = new CharacterColliderLink
            {
                Collider = characterBodyNode.Collider,
                Shape = characterColliderShape
            };

            characterBodyNode.InnerCast.TargetPosition = Vector3.Up * characterSettings.BaseHeight;
            characterBodyNode.OuterCast.TargetPosition = Vector3.Up * characterSettings.BaseHeight;

            characterRootNode.PackedEntity = _world.PackEntity(characterEntity);
            characterRootNode.World = _world;

            characterBodyNode.Position = spawn.Position;
            characterBodyNode.Rotation = spawn.Rotation;

            spawn.Parent.AddChild(characterRootNode);

            _destroyPool.Add(entity);
        }
    }
}