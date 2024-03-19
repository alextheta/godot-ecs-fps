using im.Common.LeoEcsLite;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CharacterFeature;

public class CharacterCrouchSystem : IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<CharacterBodyLink> _characterBodyLinkPool;
    [Inject] private EcsPool<CharacterSettings> _characterSettingsPool;
    [Inject] private EcsPool<CharacterMovementSpeed> _characterMovementSpeedPool;
    [Inject] private EcsPool<CharacterCrouching> _characterCrouchingPool;
    [Inject] private EcsPool<CharacterChangeCrouchState> _characterChangeCrouchStatePool;
    [Inject] private EcsPool<CharacterAdjustBodyEvent> _characterAdjustBodyEventPool;

    public void Run(IEcsSystems systems)
    {
        var characterFilter = _world.Filter<Character>()
            .Inc<CharacterBodyLink>()
            .Inc<CharacterSettings>()
            .Inc<CharacterChangeCrouchState>()
            .End();

        foreach (var characterEntity in characterFilter)
        {
            var characterBody = _characterBodyLinkPool.Get(characterEntity).Value;
            var newCrouchState = _characterChangeCrouchStatePool.Get(characterEntity).Value;

            if (newCrouchState)
            {
                _characterCrouchingPool.Set(characterEntity);
            }
            else
            {
                _characterCrouchingPool.Del(characterEntity);
            }

            characterBody.OuterCast.Enabled = newCrouchState;
            characterBody.InnerCast.Enabled = newCrouchState;

            ref var characterSettings = ref _characterSettingsPool.Get(characterEntity).Value;
            var targetHeight = newCrouchState ? characterSettings.BaseCrouchHeight : characterSettings.BaseHeight;
            _characterAdjustBodyEventPool.Set(characterEntity).Height = targetHeight;

            var moveSpeed = characterSettings.BaseSpeed * (newCrouchState ? characterSettings.CrouchMoveSpeedMultiplier : 1f);
            _characterMovementSpeedPool.Get(characterEntity).Value = moveSpeed;

            _characterChangeCrouchStatePool.Del(characterEntity);
        }
    }
}