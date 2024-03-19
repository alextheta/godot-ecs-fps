using im.Common.LeoEcsLite;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.InputFeature;

public class InputFeature : ILeoLiteEcsInitFeature, ILeoLiteEcsUpdateFeature, ILeoLiteEcsDestroyFeature
{
    [Inject] private TenjectContainer _container;

    [Inject] public EcsWorld World { get; set; }
    public IEcsSystems UpdateSystems { get; set; }

    public void Init()
    {
        UpdateSystems = new EcsSystems(World);

        UpdateSystems
            .Add(_container.ResolveNew<InputInitSystem>())
            .Add(_container.ResolveNew<InputAxisSystem>())
            .Add(_container.ResolveNew<InputButtonSystem>())
            .Add(_container.ResolveNew<InputMouseCaptureSystem>())
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