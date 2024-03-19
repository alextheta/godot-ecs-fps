using im.Common.LeoEcsLite;
using im.Features.CharacterFeature;
using im.Features.InputFeature;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.PlayerCharacterControlFeature;

public class PlayerCharacterControlJumpSystem : IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<InputButtonPressedEvent> _inputButtonPressedPool;
    [Inject] private EcsPool<CharacterControlJumpEvent> _characterControlJumpPool;

    public void Run(IEcsSystems systems)
    {
        var characterFilter = _world.Filter<Character>().Inc<CharacterPossess>().End();
        var inputButtonFilter = _world.Filter<InputButtonPressedEvent>().End();

        foreach (var inputEventEntity in inputButtonFilter)
        {
            if (_inputButtonPressedPool.Get(inputEventEntity).Action != InputAction.Jump)
            {
                continue;
            }

            foreach (var characterEntity in characterFilter)
            {
                _characterControlJumpPool.Set(characterEntity);
            }
        }
    }
}