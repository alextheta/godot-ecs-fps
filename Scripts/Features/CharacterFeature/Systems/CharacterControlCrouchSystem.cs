using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CharacterFeature;

public class CharacterControlCrouchSystem : IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<CharacterBodyLink> _characterBodyLinkPool;
    [Inject] private EcsPool<CharacterCrouching> _characterCrouchingPool;
    [Inject] private EcsPool<CharacterChangeCrouchState> _characterChangeCrouchStatePool;

    public void Run(IEcsSystems systems)
    {
        var characterFilter = _world.Filter<Character>()
            .Inc<CharacterBodyLink>()
            .Inc<CharacterControlCrouchEvent>()
            .Exc<CharacterChangeCrouchState>()
            .Exc<CharacterClimbing>()
            .End();

        foreach (var characterEntity in characterFilter)
        {
            var crouching = _characterCrouchingPool.Has(characterEntity);
            var characterBody = _characterBodyLinkPool.Get(characterEntity).Value;

            if (crouching && characterBody.FullBodyShapeCast.IsColliding())
            {
                continue;
            }

            _characterChangeCrouchStatePool.Add(characterEntity).Value = !crouching;
        }
    }
}