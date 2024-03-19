using Godot;
using im.Common.Godot;
using im.Settings;

namespace im.Features.CharacterFeature;

public partial class CharacterSpawnProvider : Node3DProvider<CharacterSpawn>
{
    [Export] private PackedScene _prefab;
    [Export] private CharacterSettingsData _settings;

    protected override void Init()
    {
        Data.Prefab = _prefab;
        Data.Parent = GetTree().Root.GetChild(0) as Node3D;
        Data.Position = Position;
        Data.Rotation = Rotation;
        Data.Settings = _settings;
    }
}