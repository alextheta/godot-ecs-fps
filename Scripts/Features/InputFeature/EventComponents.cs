using Godot;

namespace im.Features.InputFeature;

public struct InputButtonPressedEvent
{
    public InputAction Action;
}

public struct InputButtonReleasedEvent
{
    public InputAction Action;
}

public struct InputMouseCaptureChanged
{
    public Godot.Input.MouseModeEnum Mode;
}

public struct InputMouseEvent
{
    public Vector2 Position;
    public Vector2 Delta;
}