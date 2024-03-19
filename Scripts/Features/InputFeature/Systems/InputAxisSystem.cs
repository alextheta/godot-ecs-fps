using System.Collections.Generic;
using Godot;
using Leopotam.EcsLite;
using Te.DI;

namespace im.Features.InputFeature;

public class InputAxisSystem : IEcsInitSystem, IEcsRunSystem
{
    private const string LeftAxisLeftInputBinding = "leftaxis_left";
    private const string LeftAxisRightInputBinding = "leftaxis_right";
    private const string LeftAxisUpInputBinding = "leftaxis_up";
    private const string LeftAxisDownInputBinding = "leftaxis_down";
    private const string RightAxisLeftInputBinding = "rightaxis_left";
    private const string RightAxisRightInputBinding = "rightaxis_right";
    private const string RightAxisUpInputBinding = "rightaxis_up";
    private const string RightAxisDownInputBinding = "rightaxis_down";

    [Inject] private EcsWorld _world;
    [Inject] private EcsPool<InputAxis> _inputAxisPool;

    public void Init(IEcsSystems systems)
    {
        var filter = _world.Filter<Input>().End();
        foreach (var entity in filter)
        {
            ref var inputAxis = ref _inputAxisPool.Add(entity);

            inputAxis.Value = new Dictionary<AxisType, Vector2>();
            inputAxis.Config = new Dictionary<AxisType, AxisConfig>
            {
                [AxisType.Left] = new()
                {
                    Actions = new[] { LeftAxisLeftInputBinding, LeftAxisRightInputBinding, LeftAxisDownInputBinding, LeftAxisUpInputBinding }
                },
                [AxisType.Right] = new()
                {
                    Actions = new[] { RightAxisLeftInputBinding, RightAxisRightInputBinding, RightAxisDownInputBinding, RightAxisUpInputBinding }
                }
            };
        }
    }

    public void Run(IEcsSystems systems)
    {
        var filter = _world.Filter<Input>().Inc<InputAxis>().End();

        foreach (var entity in filter)
        {
            ref var inputAxis = ref _inputAxisPool.Get(entity);

            inputAxis.Value[AxisType.Left] = UpdateAxis(AxisType.Left, ref inputAxis);
            inputAxis.Value[AxisType.Right] = UpdateAxis(AxisType.Right, ref inputAxis);
        }
    }

    private static Vector2 UpdateAxis(AxisType axisType, ref InputAxis inputAxis)
    {
        return Godot.Input.GetVector(
            inputAxis.Config[axisType].Actions[(int)AxisDirection.Right],
            inputAxis.Config[axisType].Actions[(int)AxisDirection.Left],
            inputAxis.Config[axisType].Actions[(int)AxisDirection.Down],
            inputAxis.Config[axisType].Actions[(int)AxisDirection.Up]);
    }
}