using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CharacterFeature;

public class CharacterControlJumpSystem : IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<CharacterBodyLink> _characterBodyLinkPool;
    [Inject] private EcsPool<CharacterJumping> _characterJumpingPool;

    public void Run(IEcsSystems systems)
    {
        var characterFilter = _world.Filter<Character>()
            .Inc<CharacterBodyLink>()
            .Inc<CharacterSettings>()
            .Inc<CharacterControlJumpEvent>()
            .Exc<CharacterJumping>()
            .Exc<CharacterClimbing>()
            .End();

        foreach (var characterEntity in characterFilter)
        {
            var characterBody = _characterBodyLinkPool.Get(characterEntity).Value;

            if (!characterBody.IsOnFloor())
            {
                continue;
            }

            _characterJumpingPool.Add(characterEntity);
        }
    }
}