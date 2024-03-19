using System.Collections.Generic;
using Godot;

namespace im.Features.InputFeature;

public struct Input
{
}

public struct InputAxis
{
    public Dictionary<AxisType, AxisConfig> Config;
    public Dictionary<AxisType, Vector2> Value;
}

public struct InputButton
{
    public Dictionary<InputAction, ButtonState> States;
}