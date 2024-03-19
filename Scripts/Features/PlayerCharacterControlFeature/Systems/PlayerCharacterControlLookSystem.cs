using im.Common.LeoEcsLite;
using im.Features.CharacterFeature;
using im.Features.InputFeature;
using im.Settings;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.PlayerCharacterControlFeature;

public class PlayerCharacterControlLookSystem : IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<InputMouseEvent> _inputMouseEventPool;
    [Inject] private EcsPool<CharacterControlLookEvent> _characterLookControlPool;
    
    [Inject] private SettingsDatabase _settingsDatabase;

    public void Run(IEcsSystems systems)
    {
        var characterFilter = _world.Filter<Character>().Inc<CharacterPossess>().End();
        var inputMouseFilter = _world.Filter<InputMouseEvent>().End();

        foreach (var inputEventEntity in inputMouseFilter)
        {
            ref var inputMouseEvent = ref _inputMouseEventPool.Get(inputEventEntity);

            foreach (var characterEntity in characterFilter)
            {
                _characterLookControlPool.Set(characterEntity).Delta = inputMouseEvent.Delta * _settingsDatabase.ControlsSettings.MouseSensitivity;
            }
        }
    }
}