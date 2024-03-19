using System;
using Godot;
using im.Common;
using im.Common.LeoEcsLite;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CharacterFeature;

public class CharacterCrouchDynamicHeightSystem : IEcsRunSystem
{
    private const float ColliderHeightGap = 0.05f;

    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<CharacterBodyLink> _characterBodyLinkPool;
    [Inject] private EcsPool<CharacterSettings> _characterSettingsPool;
    [Inject] private EcsPool<CharacterColliderLink> _characterColliderLinkPool;
    [Inject] private EcsPool<CharacterAdjustBodyEvent> _characterAdjustBodyEventPool;

    [Inject] private Service _serviceNode;

    private readonly PhysicsTestMotionParameters3D _motionParameters = new();
    private readonly PhysicsTestMotionResult3D _motionResult = new();

    public void Run(IEcsSystems systems)
    {
        var characterFilter = _world.Filter<Character>()
            .Inc<CharacterBodyLink>()
            .Inc<CharacterSettings>()
            .Inc<CharacterColliderLink>()
            .Inc<CharacterCrouching>()
            .Exc<CharacterAdjustBodyEvent>()
            .End();

        var deltaTime = (float)_serviceNode.GetPhysicsProcessDeltaTime();

        foreach (var characterEntity in characterFilter)
        {
            var characterBody = _characterBodyLinkPool.Get(characterEntity).Value;

            var horizontalVelocity = new Vector3(characterBody.Velocity.X, 0, characterBody.Velocity.Z);
            if (horizontalVelocity == Vector3.Zero)
            {
                continue;
            }

            var characterSettings = _characterSettingsPool.Get(characterEntity).Value;

            var bodyRid = characterBody.GetRid();

            _motionParameters.From = characterBody.GlobalTransform;
            _motionParameters.Motion = horizontalVelocity * deltaTime;
            _motionParameters.Margin = 0f;

            var outerRayCast = characterBody.OuterCast;
            var innerRayCast = characterBody.InnerCast;

            var characterHeight = characterBody.GlobalPosition.Y;

            if (PhysicsServer3D.BodyTestMotion(bodyRid, _motionParameters, _motionResult))
            {
                if (!outerRayCast.IsColliding())
                {
                    continue;
                }

                var outerSurfaceHeight = outerRayCast.GetCollisionPoint().Y - characterHeight;
                if (outerSurfaceHeight < characterSettings.MinCrouchHeight)
                {
                    continue;
                }

                var innerSurfaceHeight = innerRayCast.IsColliding()
                    ? innerRayCast.GetCollisionPoint().Y - characterHeight
                    : characterSettings.BaseCrouchHeight;

                var targetHeight = Mathf.Clamp(
                    value: Mathf.Min(innerSurfaceHeight, outerSurfaceHeight) - ColliderHeightGap,
                    min: characterSettings.MinCrouchHeight,
                    max: characterSettings.BaseCrouchHeight);

                _characterAdjustBodyEventPool.Set(characterEntity).Height = targetHeight;
            }
            else
            {
                var fullBodyShapeCollision = characterBody.FullBodyShapeCast.IsColliding();
                var innerRayCastCollision = innerRayCast.IsColliding();

                ref var colliderLink = ref _characterColliderLinkPool.Get(characterEntity);
                var currentHeight = colliderLink.Shape.Height;

                if (fullBodyShapeCollision && innerRayCastCollision)
                {
                    var innerCollisionHeight = innerRayCast.GetCollisionPoint().Y;
                    if (outerRayCast.IsColliding() && innerCollisionHeight > outerRayCast.GetCollisionPoint().Y)
                    {
                        continue;
                    }

                    var innerSurfaceHeight = innerCollisionHeight - characterHeight;

                    var targetHeight = Mathf.Clamp(
                        value: Mathf.Min(characterSettings.BaseCrouchHeight, innerSurfaceHeight - ColliderHeightGap),
                        min: characterSettings.MinCrouchHeight,
                        max: characterSettings.BaseCrouchHeight);

                    var heightDiff = targetHeight - currentHeight;
                    if (Mathf.Abs(heightDiff) <= Const.FloatTolerance)
                    {
                        continue;
                    }

                    _motionParameters.Motion = new Vector3(0f, heightDiff, 0f);
                    if (PhysicsServer3D.BodyTestMotion(bodyRid, _motionParameters, _motionResult))
                    {
                        continue;
                    }

                    _characterAdjustBodyEventPool.Set(characterEntity).Height = targetHeight;
                }
                else if (!fullBodyShapeCollision && !innerRayCastCollision && Math.Abs(currentHeight - characterSettings.BaseCrouchHeight) > Const.FloatTolerance)
                {
                    _characterAdjustBodyEventPool.Set(characterEntity).Height = characterSettings.BaseCrouchHeight;
                }
            }
        }
    }
}