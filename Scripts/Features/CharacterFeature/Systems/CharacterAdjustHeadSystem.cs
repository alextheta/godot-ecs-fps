using Godot;
using im.Common;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CharacterFeature;

public class CharacterAdjustHeadSystem : IEcsRunSystem
{
    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<CharacterBodyLink> _characterBodyLinkPool;
    [Inject] private EcsPool<CharacterSettings> _characterSettingsPool;
    [Inject] private EcsPool<CharacterAdjustHead> _characterAdjustHeadPool;

    [Inject] private Service _serviceNode;

    public void Run(IEcsSystems systems)
    {
        var characterFilter = _world.Filter<Character>()
            .Inc<CharacterBodyLink>()
            .Inc<CharacterSettings>()
            .Inc<CharacterAdjustHead>()
            .End();

        var deltaTime = (float)_serviceNode.GetProcessDeltaTime();

        foreach (var characterEntity in characterFilter)
        {
            var characterBody = _characterBodyLinkPool.Get(characterEntity).Value;
            var headNode = characterBody.Head;

            var targetHeadHeight = _characterAdjustHeadPool.Get(characterEntity).Height;
            var currentHeadHeight = headNode.Position.Y;

            if (Mathf.Abs(currentHeadHeight - targetHeadHeight) > Const.FloatTolerance)
            {
                var characterSettings = _characterSettingsPool.Get(characterEntity).Value;
                var interpolatedHeight = Mathf.Lerp(currentHeadHeight, targetHeadHeight, characterSettings.HeadAdjustSmooth * deltaTime);
                headNode.Position = new Vector3(0f, interpolatedHeight, 0f);
            }
            else
            {
                headNode.Position = new Vector3(0, targetHeadHeight, 0);
                _characterAdjustHeadPool.Del(characterEntity);
            }
        }
    }
}