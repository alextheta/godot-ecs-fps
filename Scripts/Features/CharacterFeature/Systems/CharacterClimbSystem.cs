using im.Common.LeoEcsLite;
using im.Features.PhysicsFeature;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CharacterFeature;

public class CharacterClimbSystem : IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<CharacterBodyLink> _characterBodyLinkPool;
    [Inject] private EcsPool<CharacterSettings> _characterSettingsPool;
    [Inject] private EcsPool<CharacterClimbing> _characterClimbingPool;
    [Inject] private EcsPool<CharacterChangeCollisionLayersEvent> _characterChangeCollisionLayersEventPool;

    [Inject] private PhysicsUtil.CollisionLayer _collisionLayerUtil;

    [Inject] private Service _serviceNode;

    public void Run(IEcsSystems systems)
    {
        var characterFilter = _world.Filter<Character>()
            .Inc<CharacterBodyLink>()
            .Inc<CharacterSettings>()
            .Inc<CharacterClimbing>()
            .End();

        var deltaTime = (float)_serviceNode.GetPhysicsProcessDeltaTime();

        foreach (var characterEntity in characterFilter)
        {
            var characterBody = _characterBodyLinkPool.Get(characterEntity).Value;
            var characterSettings = _characterSettingsPool.Get(characterEntity).Value;
            ref var climbing = ref _characterClimbingPool.Get(characterEntity);

            characterBody.Position = characterBody.Position.Lerp(climbing.TargetPosition, characterSettings.ClimbLerpSmooth * deltaTime);

            climbing.ClimbTime += deltaTime;
            if (climbing.ClimbTime < characterSettings.ClimbMaxTime && characterBody.Position.DistanceTo(climbing.TargetPosition) > characterSettings.ClimbCompletionDistanceCheck)
            {
                continue;
            }

            _characterChangeCollisionLayersEventPool.Set(characterEntity) = new CharacterChangeCollisionLayersEvent
            {
                EnableLayers = _collisionLayerUtil.GetGroupLayers(CollisionLayerGroup.Environment)
            };

            _characterClimbingPool.Del(characterEntity);
        }
    }
}