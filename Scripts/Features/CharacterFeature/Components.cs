using Godot;
using im.Settings;

namespace im.Features.CharacterFeature;

public struct CharacterSpawn
{
    public PackedScene Prefab;
    public Node3D Parent;
    public Vector3 Position;
    public Vector3 Rotation;
    public CharacterSettingsData Settings;
}

public struct Character
{
}

public struct CharacterSettings
{
    public CharacterSettingsData Value;
}

public struct CharacterPossess
{
}

public struct CharacterBodyLink
{
    public CharacterBody Value;
}

public struct CharacterColliderLink
{
    public CollisionShape3D Collider;
    public CapsuleShape3D Shape;
}

public struct CharacterMovementSpeed
{
    public float Value;
}

public struct CharacterSmoothMovement
{
    public Vector3 Direction;
}

public struct CharacterClimbing
{
    public Vector3 TargetPosition;
    public double ClimbTime;
}

public struct CharacterJumping
{
}

public struct CharacterCrouching
{
}

public struct CharacterAdjustHead
{
    public float Height;
}

public struct CharacterChangeCrouchState
{
    public bool Value;
}