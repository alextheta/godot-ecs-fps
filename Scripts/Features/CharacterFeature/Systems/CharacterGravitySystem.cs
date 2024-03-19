using Godot;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CharacterFeature;

public class CharacterGravitySystem : IEcsRunSystem
{
    private const string GravitySettingsPath = "physics/3d/default_gravity";
    private readonly float _gravity = ProjectSettings.GetSetting(GravitySettingsPath).AsSingle();

    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<CharacterBodyLink> _characterBodyLinkPool;
    [Inject] private EcsPool<CharacterSettings> _characterSettingsPool;

    public void Run(IEcsSystems systems)
    {
        var characterFilter = _world.Filter<Character>()
            .Inc<CharacterBodyLink>()
            .Inc<CharacterSettings>()
            .Exc<CharacterClimbing>()
            .End();

        foreach (var characterEntity in characterFilter)
        {
            var characterBody = _characterBodyLinkPool.Get(characterEntity).Value;

            if (characterBody.IsOnFloor())
            {
                continue;
            }

            var characterSettings = _characterSettingsPool.Get(characterEntity).Value;
            var deltaTime = (float)characterBody.GetPhysicsProcessDeltaTime();
            characterBody.Velocity -= Vector3.Up * Mathf.Pow(_gravity, characterSettings.GravityPower) * deltaTime;
        }
    }
}