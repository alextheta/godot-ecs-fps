using Godot;
using im.Common.LeoEcsLite;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CharacterFeature;

public class CharacterUpdateHeightSystem : IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<CharacterSettings> _characterSettingsPool;
    [Inject] private EcsPool<CharacterColliderLink> _characterColliderLinkPool;
    [Inject] private EcsPool<CharacterAdjustBodyEvent> _characterAdjustBodyEventPool;
    [Inject] private EcsPool<CharacterAdjustHead> _characterAdjustHeadPool;

    public void Run(IEcsSystems systems)
    {
        var characterFilter = _world.Filter<Character>()
            .Inc<CharacterSettings>()
            .Inc<CharacterColliderLink>()
            .Inc<CharacterAdjustBodyEvent>()
            .End();

        foreach (var characterEntity in characterFilter)
        {
            var characterSettings = _characterSettingsPool.Get(characterEntity).Value;
            ref var colliderLink = ref _characterColliderLinkPool.Get(characterEntity);
            var collider = colliderLink.Collider;
            var shape = colliderLink.Shape;

            ref var targetHeight = ref _characterAdjustBodyEventPool.Get(characterEntity).Height;

            shape.Height = targetHeight;
            collider.Position = new Vector3(collider.Position.X, targetHeight / 2f, collider.Position.Z);

            _characterAdjustHeadPool.Set(characterEntity) = new CharacterAdjustHead
            {
                Height = shape.Height * (1f - characterSettings.HeadOffsetPercent)
            };
        }
    }
}