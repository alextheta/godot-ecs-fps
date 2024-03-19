using System.Collections.Generic;
using Godot;
using Te.DI;

namespace im.Features.PhysicsFeature;

public static class PhysicsUtil
{
    public class Cast
    {
        private const string PositionVectorKey = "position";

        [Inject] private PhysicsDirectSpaceState3D _spaceState;

        private readonly PhysicsRayQueryParameters3D _castParameters = new();

        public bool PhysicsCastRay(Vector3 from, Vector3 to, out Vector3 hitPosition)
        {
            _castParameters.From = from;
            _castParameters.To = to;

            var result = _spaceState.IntersectRay(_castParameters);
            if (result.Count == 0)
            {
                hitPosition = Vector3.Zero;
                return false;
            }

            hitPosition = result[PositionVectorKey].AsVector3();
            return true;
        }

        public bool PhysicsCastRay(Vector3 from, Vector3 to)
        {
            return PhysicsCastRay(from, to, out _);
        }
    }

    public class CollisionLayer
    {
        private const string GodotLayerSettingsPath = "layer_names/3d_physics/layer_";
        private const int GodotLayerCount = 32;

        private const string LayerCharacters = "Characters";
        private const string LayerStaticObjects = "StaticObjects";
        private const string LayerDynamicObjects = "DynamicObjects";

        private readonly Dictionary<string, uint> _layerMap;
        private readonly Dictionary<CollisionLayerGroup, List<string>> _layerGroups;

        public CollisionLayer()
        {
            _layerMap = new Dictionary<string, uint>();

            for (var i = 0u; i < GodotLayerCount; i++)
            {
                var layerName = ProjectSettings.GetSetting($"{GodotLayerSettingsPath}{i}").ToString();
                if (string.IsNullOrEmpty(layerName))
                {
                    continue;
                }

                _layerMap.Add(layerName, i);
            }

            _layerGroups = new Dictionary<CollisionLayerGroup, List<string>>
            {
                [CollisionLayerGroup.Environment] = new()
                {
                    LayerStaticObjects,
                    LayerDynamicObjects
                },
            };
        }

        public uint GetLayer(string layerName)
        {
            return _layerMap[layerName];
        }

        public List<string> GetGroupLayers(CollisionLayerGroup group)
        {
            return _layerGroups.GetValueOrDefault(group);
        }
    }
}