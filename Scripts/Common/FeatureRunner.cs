using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Te.DI;
using Te.EcsFeatureRunner;

namespace im.Common;

public class FeatureRunner<TEcsWorld, TEcsComponentContainer> : EcsFeatureRunner where TEcsWorld : class where TEcsComponentContainer : class
{
    private readonly TenjectContainer _container;

    public FeatureRunner(TEcsWorld world, TenjectContainer container)
    {
        _container = container;
        InjectWorld(world);
        InjectPools(world);
    }

    public override IEcsFeatureRunner AddFeature<T>()
    {
        var feature = _container.ResolveNew<T>();
        return base.AddFeature(feature);
    }

    public override IEcsFeatureRunner AddFeature<T>(T feature)
    {
        _container.ResolveInstance(feature);
        return base.AddFeature(feature);
    }

    private void InjectWorld(TEcsWorld world)
    {
        _container.BindInstance(world);
    }

    private void InjectPools(TEcsWorld world)
    {
        var ecsPoolType = typeof(TEcsComponentContainer);

        var getContainerMethod = world
            .GetType()
            .GetMethods()
            .Where(methodInfo => methodInfo.IsGenericMethod && methodInfo.ReturnType.IsGenericType)
            .FirstOrDefault(methodInfo => methodInfo.ReturnType.IsAssignableTo(ecsPoolType));

        if (getContainerMethod == null)
        {
            throw new Exception($"Generic method to obtain component container \"{ecsPoolType}\" is not found in \"{typeof(TEcsWorld)}\" class");
        }

        var genericPoolFields = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .SelectMany(type => type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            .Where(info => info.IsDefined(InjectAttribute.Type))
            .Where(fieldInfo => fieldInfo.FieldType.IsAssignableTo(ecsPoolType));

        var poolTypeCache = new HashSet<Type>();

        foreach (var fieldInfo in genericPoolFields)
        {
            var fieldType = fieldInfo.FieldType;
            var poolType = fieldType.GetGenericArguments().FirstOrDefault();
            if (poolType == null)
            {
                throw new Exception($"Generic component container type is null for \"{fieldType}\"");
            }

            if (poolTypeCache.Contains(poolType))
            {
                continue;
            }

            var genericGetPoolMethod = getContainerMethod.MakeGenericMethod(poolType);
            var pool = genericGetPoolMethod.Invoke(world, null) as TEcsComponentContainer;
            _container.BindInstance(fieldType, pool);
            poolTypeCache.Add(poolType);
        }
    }
}