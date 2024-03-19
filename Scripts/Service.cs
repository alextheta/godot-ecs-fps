using Godot;
using im.Common;
using im.Common.Godot;
using im.Features.CharacterFeature;
using im.Features.CleanupFeature;
using im.Features.DebugFeature;
using im.Features.InputFeature;
using im.Features.PhysicsFeature;
using im.Features.PlayerCharacterControlFeature;
using im.Settings;
using Leopotam.EcsLite;
using Te.DI;
using Te.EcsFeatureRunner;

namespace im;

public partial class Service : Node
{
    private readonly TenjectContainer _container = new(bindSelf: true);

    [Export] private SettingsDatabase _settingsDatabase;

    private IEcsFeatureRunner _featureRunner;

    private void AddFeatures()
    {
        _featureRunner
            .AddFeature<InputFeature>()
            .AddFeature<PlayerCharacterControlFeature>()
            .AddFeature<CharacterFeature>()
            .AddFeature<DebugFeature>()
            .AddFeature<CleanupFeature>() // Must be last
            ;
    }

    public override void _Ready()
    {
        var worldProvider = GetNode<EcsWorldProvider>(EcsWorldProvider.NodePath);
        _featureRunner = new FeatureRunner<EcsWorld, IEcsPool>(worldProvider.EcsWorld, _container);

        InjectInfrastructure();
        InjectSettings();
        InjectUtils();

        AddFeatures();

        _featureRunner.Init();
    }

    public override void _Process(double delta)
    {
        _featureRunner.Update();
    }

    public override void _PhysicsProcess(double delta)
    {
        _featureRunner.PhysicsUpdate();
    }

    private void InjectInfrastructure()
    {
        var sceneTree = GetTree();
        var world3D = (sceneTree.CurrentScene as Node3D)?.GetWorld3D();
        var spaceState = world3D.DirectSpaceState;

        _container.BindInstance(sceneTree);
        _container.BindInstance(world3D);
        _container.BindInstance(spaceState);
        _container.BindInstance(_featureRunner);
        _container.BindInstance(this);
    }

    private void InjectSettings()
    {
        _container.BindInstance(_settingsDatabase);
    }

    private void InjectUtils()
    {
        _container.BindNew<PhysicsUtil.Cast>();
        _container.BindNew<PhysicsUtil.CollisionLayer>();
    }
}