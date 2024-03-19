using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CharacterFeature;

public class CharacterBodyProcessSystem : IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<CharacterBodyLink> _characterBodyLinkPool;

    public void Run(IEcsSystems systems)
    {
        var characterFilter = _world.Filter<Character>()
            .Inc<CharacterBodyLink>()
            .End();

        foreach (var characterEntity in characterFilter)
        {
            var characterBody = _characterBodyLinkPool.Get(characterEntity).Value;
            characterBody.MoveAndSlide();
        }
    }
}