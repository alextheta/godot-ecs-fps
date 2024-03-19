using im.Common.LeoEcsLite;
using im.Features.CharacterFeature;
using im.Features.InputFeature;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CleanupFeature;

public class CleanupFeature : ILeoLiteEcsInitFeature, ILeoLiteEcsUpdateFeature, ILeoLiteEcsDestroyFeature
{
    [Inject] private TenjectContainer _container;

    [Inject] public EcsWorld World { get; set; }
    public IEcsSystems UpdateSystems { get; set; }

    public void Init()
    {
        UpdateSystems = new EcsSystems(World);

        UpdateSystems
            .Add(_container.ResolveNew<CleanupEventFeature<InputButtonPressedEvent>>())
            .Add(_container.ResolveNew<CleanupEventFeature<InputButtonReleasedEvent>>())
            .Add(_container.ResolveNew<CleanupEventFeature<InputMouseEvent>>())
            .Add(_container.ResolveNew<CleanupEventFeature<CharacterControlJumpEvent>>())
            .Add(_container.ResolveNew<CleanupEventFeature<CharacterControlLookEvent>>())
            .Add(_container.ResolveNew<CleanupEventFeature<CharacterControlMoveEvent>>())
            .Add(_container.ResolveNew<CleanupEventFeature<CharacterControlCrouchEvent>>())
            .Add(_container.ResolveNew<CleanupEventFeature<CharacterAdjustBodyEvent>>())
            .Add(_container.ResolveNew<CleanupEventFeature<CharacterChangeCollisionLayersEvent>>())
            .Add(_container.ResolveNew<CleanupNode3DSystem>())
            .Add(_container.ResolveNew<CleanupEntitySystem>()) // Must be last
            ;

        UpdateSystems.Init();
    }

    public void Update()
    {
        UpdateSystems.Run();
    }

    public void Destroy()
    {
        UpdateSystems.Destroy();
    }
}