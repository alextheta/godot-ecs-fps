using Leopotam.EcsLite;
using Te.EcsFeatureRunner.Feature;

namespace im.Common.LeoEcsLite;

public interface ILeoLiteEcsInitFeature : IEcsInitFeature<EcsWorld>
{
}

public interface ILeoLiteEcsUpdateFeature : IEcsUpdateFeature<EcsWorld, IEcsSystems>
{
}

public interface ILeoLiteEcsPhysicsUpdateFeature : IEcsPhysicsUpdateFeature<EcsWorld, IEcsSystems>
{
}

public interface ILeoLiteEcsDestroyFeature : IEcsDestroyFeature<EcsWorld>
{
}