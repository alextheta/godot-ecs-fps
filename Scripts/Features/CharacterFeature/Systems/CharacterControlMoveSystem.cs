using Godot;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CharacterFeature;

public class CharacterControlMoveSystem : IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<CharacterBodyLink> _characterBodyLinkPool;
    [Inject] private EcsPool<CharacterSettings> _characterSettingsPool;
    [Inject] private EcsPool<CharacterMovementSpeed> _characterMovementSpeedPool;
    [Inject] private EcsPool<CharacterSmoothMovement> _characterSmoothMovementPool;
    [Inject] private EcsPool<CharacterControlMoveEvent> _characterControlMoveEventPool;
    [Inject] private EcsPool<CharacterCrouching> _characterCrouchingPool;

    [Inject] private Service _serviceNode;

    public void Run(IEcsSystems systems)
    {
        var characterFilter = _world.Filter<Character>()
            .Inc<CharacterBodyLink>()
            .Inc<CharacterSettings>()
            .Inc<CharacterMovementSpeed>()
            .Inc<CharacterSmoothMovement>()
            .Inc<CharacterControlMoveEvent>()
            .Exc<CharacterClimbing>()
            .End();

        var deltaTime = (float)_serviceNode.GetProcessDeltaTime();

        foreach (var characterEntity in characterFilter)
        {
            var characterBody = _characterBodyLinkPool.Get(characterEntity).Value;
            var characterSettings = _characterSettingsPool.Get(characterEntity).Value;
            ref var moveControl = ref _characterControlMoveEventPool.Get(characterEntity);
            ref var smoothMovement = ref _characterSmoothMovementPool.Get(characterEntity);

            var inputDirection = new Vector3(moveControl.Direction.X, 0, moveControl.Direction.Y);
            var movementDirection = (characterBody.Transform.Basis * inputDirection).Normalized();

            smoothMovement.Direction = smoothMovement.Direction.Lerp(movementDirection, deltaTime * characterSettings.MovementSmoothInterpolation);
            var movementSpeed = _characterMovementSpeedPool.Get(characterEntity).Value;

            if (_characterCrouchingPool.Has(characterEntity) && movementDirection != Vector3.Zero)
            {
                var crouchRayCast = characterBody.OuterCast;
                crouchRayCast.Position = new Vector3
                {
                    X = inputDirection.X * characterSettings.CrouchHeightCastOffset,
                    Y = crouchRayCast.Position.Y,
                    Z = inputDirection.Z * characterSettings.CrouchHeightCastOffset
                };
            }

            Vector3 velocity;
            if (smoothMovement.Direction != Vector3.Zero)
            {
                velocity = new Vector3
                {
                    X = smoothMovement.Direction.X * movementSpeed,
                    Y = characterBody.Velocity.Y,
                    Z = smoothMovement.Direction.Z * movementSpeed
                };
            }
            else
            {
                velocity = new Vector3
                {
                    X = Mathf.MoveToward(characterBody.Velocity.X, 0, movementSpeed),
                    Y = characterBody.Velocity.Y,
                    Z = Mathf.MoveToward(characterBody.Velocity.Z, 0, movementSpeed)
                };
            }

            characterBody.Velocity = velocity;
        }
    }
}