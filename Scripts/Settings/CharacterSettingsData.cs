using Godot;

namespace im.Settings;

public partial class CharacterSettingsData : Resource
{
    [ExportGroup("Movement")]
    [Export] public float BaseHeight;
    [Export] public float BaseSpeed;
    [Export] public float MovementSmoothInterpolation;
    [ExportGroup("Jump")]
    [Export] public float BaseJumpVelocity;
    [Export] public float GravityPower;
    [ExportGroup("Head")]
    [Export] public float HeadOffsetPercent = 0.2f;
    [Export] public float HeadAdjustSmooth;
    [ExportGroup("Crouch")]
    [Export] public float CrouchMoveSpeedMultiplier;
    [Export] public float CrouchHeightCastOffset;
    [Export] public float BaseCrouchHeight;
    [Export] public float MinCrouchHeight;
    [ExportGroup("Climb")]
    [Export] public float ClimbCompletionDistanceCheck;
    [Export] public float ClimbCheckRayLength;
    [Export] public float ClimbMaxTime;
    [Export] public float ClimbLerpSmooth;
    [ExportGroup("Step")]
    [Export] public float StepHeight = 0.3f;
}