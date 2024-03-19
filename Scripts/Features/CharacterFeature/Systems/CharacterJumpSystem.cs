using Godot;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CharacterFeature;

public class CharacterJumpSystem : IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<CharacterBodyLink> _characterBodyLinkPool;
    [Inject] private EcsPool<CharacterSettings> _characterSettingsPool;
    [Inject] private EcsPool<CharacterJumping> _characterJumpingPool;

    public void Run(IEcsSystems systems)
    {
        var characterFilter = _world.Filter<Character>()
            .Inc<CharacterBodyLink>()
            .Inc<CharacterSettings>()
            .Inc<CharacterJumping>()
            .End();

        foreach (var characterEntity in characterFilter)
        {
            var characterBody = _characterBodyLinkPool.Get(characterEntity).Value;
            var characterSettings = _characterSettingsPool.Get(characterEntity).Value;

            characterBody.Velocity = Vector3.Up * characterSettings.BaseJumpVelocity;

            _characterJumpingPool.Del(characterEntity);
        }
    }
}