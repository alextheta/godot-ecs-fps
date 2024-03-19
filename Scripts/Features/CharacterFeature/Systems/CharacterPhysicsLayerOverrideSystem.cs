using im.Features.PhysicsFeature;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CharacterFeature;

public class CharacterPhysicsLayerOverrideSystem : IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<CharacterBodyLink> _characterBodyLinkPool;
    [Inject] private EcsPool<CharacterChangeCollisionLayersEvent> _characterChangeCollisionLayersEventPool;

    [Inject] private PhysicsUtil.CollisionLayer _collisionLayerUtil;

    public void Run(IEcsSystems systems)
    {
        var characterFilter = _world.Filter<Character>()
            .Inc<CharacterBodyLink>()
            .Inc<CharacterChangeCollisionLayersEvent>()
            .End();

        foreach (var characterEntity in characterFilter)
        {
            var characterBody = _characterBodyLinkPool.Get(characterEntity).Value;
            ref var changeLayers = ref _characterChangeCollisionLayersEventPool.Get(characterEntity);

            if (changeLayers.DisableLayers != null)
            {
                foreach (var layer in changeLayers.DisableLayers)
                {
                    characterBody.CollisionMask &= ~_collisionLayerUtil.GetLayer(layer);
                }
            }

            if (changeLayers.EnableLayers == null)
            {
                continue;
            }

            foreach (var layer in changeLayers.EnableLayers)
            {
                characterBody.CollisionMask |= _collisionLayerUtil.GetLayer(layer);
            }
        }
    }
}