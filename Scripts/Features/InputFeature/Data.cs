namespace im.Features.InputFeature;

public enum InputAction
{
    Escape = 0,
    LMB = 1,
    RMB = 2,
    Jump = 3,
    Crouch = 4
}

public static class InputBinding
{
    public static readonly string[] Actions =
    {
        "escape",
        "lmb",
        "rmb",
        "jump",
        "crouch"
    };
}

public enum AxisType
{
    Left,
    Right
}

public enum AxisDirection
{
    Left = 0,
    Right = 1,
    Down = 2,
    Up = 3
}

public class AxisConfig
{
    public string[] Actions;
}

public enum ButtonState
{
    None,
    Pressed,
    Released
}