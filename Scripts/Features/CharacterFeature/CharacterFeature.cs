using im.Common.LeoEcsLite;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.CharacterFeature;

public class CharacterFeature : ILeoLiteEcsInitFeature, ILeoLiteEcsPhysicsUpdateFeature, ILeoLiteEcsUpdateFeature, ILeoLiteEcsDestroyFeature
{
    [Inject] private TenjectContainer _container;

    [Inject] public EcsWorld World { get; set; }

    public IEcsSystems UpdateSystems { get; set; }
    public IEcsSystems PhysicsUpdateSystems { get; set; }

    public void Init()
    {
        UpdateSystems = new EcsSystems(World);
        PhysicsUpdateSystems = new EcsSystems(World);

        UpdateSystems
            .Add(_container.ResolveNew<CharacterSpawnSystem>())
            .Add(_container.ResolveNew<CharacterControlClimbSystem>())
            .Add(_container.ResolveNew<CharacterControlJumpSystem>())
            .Add(_container.ResolveNew<CharacterControlCrouchSystem>())
            .Add(_container.ResolveNew<CharacterControlLookSystem>())
            .Add(_container.ResolveNew<CharacterControlMoveSystem>())
            .Add(_container.ResolveNew<CharacterCrouchSystem>())
            .Add(_container.ResolveNew<CharacterCrouchDynamicHeightSystem>())
            .Add(_container.ResolveNew<CharacterUpdateHeightSystem>())
            .Add(_container.ResolveNew<CharacterAdjustHeadSystem>())
            .Add(_container.ResolveNew<CharacterPhysicsLayerOverrideSystem>())
            ;

        PhysicsUpdateSystems
            .Add(_container.ResolveNew<CharacterClimbSystem>())
            .Add(_container.ResolveNew<CharacterJumpSystem>())
            .Add(_container.ResolveNew<CharacterGravitySystem>())
            .Add(_container.ResolveNew<CharacterStairSystem>())
            .Add(_container.ResolveNew<CharacterBodyProcessSystem>())
            ;

        UpdateSystems.Init();
        PhysicsUpdateSystems.Init();
    }

    public void Update()
    {
        UpdateSystems.Run();
    }

    public void PhysicsUpdate()
    {
        PhysicsUpdateSystems.Run();
    }

    public void Destroy()
    {
        UpdateSystems.Destroy();
        PhysicsUpdateSystems.Destroy();
    }
}