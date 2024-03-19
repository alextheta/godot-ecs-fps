using im.Common.LeoEcsLite;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.PlayerCharacterControlFeature;

public class PlayerCharacterControlFeature : ILeoLiteEcsInitFeature, ILeoLiteEcsUpdateFeature, ILeoLiteEcsDestroyFeature
{
    [Inject] private TenjectContainer _container;

    [Inject] public EcsWorld World { get; set; }
    public IEcsSystems UpdateSystems { get; set; }

    public void Init()
    {
        UpdateSystems = new EcsSystems(World);

        UpdateSystems
            .Add(_container.ResolveNew<PlayerCharacterControlLookSystem>())
            .Add(_container.ResolveNew<PlayerCharacterControlMoveSystem>())
            .Add(_container.ResolveNew<PlayerCharacterControlJumpSystem>())
            .Add(_container.ResolveNew<PlayerCharacterControlCrouchSystem>())
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