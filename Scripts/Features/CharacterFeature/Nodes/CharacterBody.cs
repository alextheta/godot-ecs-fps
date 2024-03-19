using Godot;

namespace im.Features.CharacterFeature;

public partial class CharacterBody : CharacterBody3D
{
    [ExportGroup("Structure")]
    [Export] public Node3D Head;
    [Export] public CollisionShape3D Collider;
    [Export] public ShapeCast3D FullBodyShapeCast;
    [Export] public RayCast3D InnerCast;
    [Export] public RayCast3D OuterCast;
    [Export] public Node3D ClimbCastPoint;
}