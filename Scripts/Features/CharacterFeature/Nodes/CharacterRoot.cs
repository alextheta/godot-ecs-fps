using Godot;
using im.Common.Godot;

namespace im.Features.CharacterFeature;

public partial class CharacterRoot : Node3DRoot
{
    [Export] public CharacterBody CharacterBody;
}