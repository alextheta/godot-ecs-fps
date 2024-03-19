using Godot;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CharacterFeature;

public class CharacterControlLookSystem : IEcsRunSystem
{
    private const float MaxVerticalLookAngle = 90f;

    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<CharacterBodyLink> _characterBodyLinkPool;
    [Inject] private EcsPool<CharacterControlLookEvent> _characterLookControlPool;

    [Inject] private Service _serviceNode;

    public void Run(IEcsSystems systems)
    {
        var characterFilter = _world.Filter<Character>()
            .Inc<CharacterBodyLink>()
            .Inc<CharacterControlLookEvent>()
            .End();

        var deltaTime = (float)_serviceNode.GetProcessDeltaTime();

        foreach (var characterEntity in characterFilter)
        {
            ref var control = ref _characterLookControlPool.Get(characterEntity);
            var characterBody = _characterBodyLinkPool.Get(characterEntity).Value;

            var headNode = characterBody.Head;
            var headRotation = headNode.RotationDegrees.X + control.Delta.Y * deltaTime;
            var clampedHeadRotation = Mathf.Clamp(headRotation, -MaxVerticalLookAngle, MaxVerticalLookAngle);
            headNode.RotationDegrees = new Vector3(clampedHeadRotation, 0f, 0f);

            var bodyRotationDelta = Mathf.DegToRad(control.Delta.X) * deltaTime;
            characterBody.RotateY(-bodyRotationDelta);
        }
    }
}