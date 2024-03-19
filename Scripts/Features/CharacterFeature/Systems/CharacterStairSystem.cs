using Godot;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CharacterFeature;

public class CharacterStairSystem : IEcsInitSystem, IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<CharacterBodyLink> _characterBodyLinkPool;
    [Inject] private EcsPool<CharacterSettings> _characterSettingsPool;

    [Inject] private Service _serviceNode;

    private readonly PhysicsTestMotionParameters3D _motionParameters = new();
    private readonly PhysicsTestMotionResult3D _motionResult = new();

    public void Init(IEcsSystems systems)
    {
        _motionParameters.Margin = 0f;
    }

    public void Run(IEcsSystems systems)
    {
        var characterFilter = _world.Filter<Character>()
            .Inc<CharacterBodyLink>()
            .Inc<CharacterSettings>()
            .Exc<CharacterCrouching>()
            .Exc<CharacterClimbing>()
            .End();

        var deltaTime = (float)_serviceNode.GetPhysicsProcessDeltaTime();

        foreach (var characterEntity in characterFilter)
        {
            var characterBody = _characterBodyLinkPool.Get(characterEntity).Value;

            if (!characterBody.IsOnFloor())
            {
                continue;
            }

            var horizontalVelocity = new Vector3(characterBody.Velocity.X, 0, characterBody.Velocity.Z);
            if (horizontalVelocity == Vector3.Zero)
            {
                continue;
            }

            var bodyRid = characterBody.GetRid();
            var distance = horizontalVelocity * deltaTime;

            _motionParameters.From = characterBody.GlobalTransform;
            _motionParameters.Motion = distance;

            if (!PhysicsServer3D.BodyTestMotion(bodyRid, _motionParameters, _motionResult))
            {
                continue;
            }

            var characterSettings = _characterSettingsPool.Get(characterEntity).Value;

            var stageTravel = _motionResult.GetTravel();
            var totalTravel = stageTravel;

            _motionParameters.From = _motionParameters.From.Translated(stageTravel);
            _motionParameters.Motion = characterSettings.StepHeight * Vector3.Up;
            PhysicsServer3D.BodyTestMotion(bodyRid, _motionParameters, _motionResult);

            stageTravel = _motionResult.GetTravel();
            totalTravel += stageTravel;
            var stepHeight = _motionResult.GetTravel().Length();

            _motionParameters.From = _motionParameters.From.Translated(stageTravel);
            _motionParameters.Motion = distance;
            PhysicsServer3D.BodyTestMotion(bodyRid, _motionParameters, _motionResult);

            stageTravel = _motionResult.GetTravel();
            totalTravel += stageTravel;

            _motionParameters.From = _motionParameters.From.Translated(stageTravel);
            _motionParameters.Motion = Vector3.Down * stepHeight;
            if (PhysicsServer3D.BodyTestMotion(bodyRid, _motionParameters, _motionResult))
            {
                var surfaceNormal = _motionResult.GetCollisionNormal();
                if (surfaceNormal.AngleTo(Vector3.Up) > characterBody.FloorMaxAngle)
                {
                    continue;
                }
            }

            totalTravel += _motionResult.GetTravel();

            characterBody.GlobalPosition = new Vector3
            {
                X = characterBody.GlobalPosition.X,
                Y = characterBody.GlobalPosition.Y + totalTravel.Y,
                Z = characterBody.GlobalPosition.Z
            };
        }
    }
}