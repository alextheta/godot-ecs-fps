using System.Collections.Generic;
using Godot;

namespace im.Features.CharacterFeature;

public struct CharacterControlJumpEvent
{
}

public struct CharacterControlLookEvent
{
    public Vector2 Delta;
}

public struct CharacterControlMoveEvent
{
    public Vector2 Direction;
}

public struct CharacterControlCrouchEvent
{
}

public struct CharacterAdjustBodyEvent
{
    public float Height;
}

public struct CharacterChangeCollisionLayersEvent
{
    public List<string> EnableLayers;
    public List<string> DisableLayers;
}