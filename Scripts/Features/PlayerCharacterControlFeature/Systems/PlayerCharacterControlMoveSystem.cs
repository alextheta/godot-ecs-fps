using im.Common.LeoEcsLite;
using im.Features.CharacterFeature;
using im.Features.InputFeature;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.PlayerCharacterControlFeature;

public class PlayerCharacterControlMoveSystem : IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<InputAxis> _inputAxisPool;
    [Inject] private EcsPool<CharacterControlMoveEvent> _characterMoveControlPool;

    public void Run(IEcsSystems systems)
    {
        var characterFilter = _world.Filter<Character>().Inc<CharacterPossess>().End();
        var inputButtonFilter = _world.Filter<InputButton>().End();

        foreach (var inputAxisEntity in inputButtonFilter)
        {
            ref var inputAxis = ref _inputAxisPool.Get(inputAxisEntity);

            foreach (var characterEntity in characterFilter)
            {
                _characterMoveControlPool.Set(characterEntity) = new CharacterControlMoveEvent
                {
                    Direction = inputAxis.Value[AxisType.Left]
                };
            }
        }
    }
}