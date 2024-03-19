using Godot;
using im.Features.PhysicsFeature;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CharacterFeature;

public class CharacterControlClimbSystem : IEcsInitSystem, IEcsRunSystem
{
    private const float ClimbCastMargin = 0.01f;

    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<CharacterBodyLink> _characterBodyLinkPool;
    [Inject] private EcsPool<CharacterSettings> _characterSettingsPool;
    [Inject] private EcsPool<CharacterClimbing> _characterClimbingPool;
    [Inject] private EcsPool<CharacterChangeCrouchState> _characterChangeCrouchStatePool;
    [Inject] private EcsPool<CharacterChangeCollisionLayersEvent> _characterChangeCollisionLayersEventPool;

    [Inject] private PhysicsUtil.CollisionLayer _collisionLayerUtil;
    [Inject] private PhysicsUtil.Cast _castUtil;

    private Vector3 _climbCastMarginVector;

    public void Init(IEcsSystems systems)
    {
        _climbCastMarginVector = new Vector3(0f, ClimbCastMargin, 0f);
    }

    public void Run(IEcsSystems systems)
    {
        var characterFilter = _world.Filter<Character>()
            .Inc<CharacterBodyLink>()
            .Inc<CharacterSettings>()
            .Inc<CharacterControlJumpEvent>()
            .Exc<CharacterClimbing>()
            .End();

        foreach (var characterEntity in characterFilter)
        {
            var characterBody = _characterBodyLinkPool.Get(characterEntity).Value;
            if (characterBody.InnerCast.IsColliding())
            {
                continue;
            }

            ref var characterSettings = ref _characterSettingsPool.Get(characterEntity).Value;

            var downcastStartPosition = characterBody.ClimbCastPoint.GlobalPosition;
            if (!_castUtil.PhysicsCastRay(downcastStartPosition, downcastStartPosition + Vector3.Down * characterSettings.ClimbCheckRayLength, out var targetClimbPosition))
            {
                continue;
            }

            var upcastStartPosition = targetClimbPosition + _climbCastMarginVector;
            if (_castUtil.PhysicsCastRay(upcastStartPosition, targetClimbPosition + Vector3.Up * characterSettings.BaseHeight))
            {
                if (_castUtil.PhysicsCastRay(upcastStartPosition, targetClimbPosition + Vector3.Up * characterSettings.MinCrouchHeight))
                {
                    continue;
                }

                _characterChangeCrouchStatePool.Add(characterEntity).Value = true;
            }

            _characterChangeCollisionLayersEventPool.Add(characterEntity) = new CharacterChangeCollisionLayersEvent
            {
                DisableLayers = _collisionLayerUtil.GetGroupLayers(CollisionLayerGroup.Environment)
            };

            _characterClimbingPool.Add(characterEntity) = new CharacterClimbing
            {
                TargetPosition = targetClimbPosition,
                ClimbTime = 0f
            };
        }
    }
}