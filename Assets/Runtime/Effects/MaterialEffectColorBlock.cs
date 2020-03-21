using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyMap.Effects
{
    [Serializable]
    public class MaterialEffectColorBlock
    {
        [SerializeField] private string _materialProperty;
        [SerializeField] private Color _unavailable = Color.black;
        [SerializeField] private Color _available = Color.grey;
        [SerializeField] private Color _highlighted = Color.white;
        [SerializeField] private Color _currentView = Color.white;

        private HashSet<Material> _validMaterials = new HashSet<Material>();

        public void Bind(Material mat, MapEffectState state)
        {
            if (!Validate(mat)) return;
            var color = GetColor(state);
            mat.SetColor(_materialProperty, color);
        }

        private bool Validate(Material mat)
        {
            // if Validation already cached - use
            if (_validMaterials.Contains(mat)) return true;

            if (string.IsNullOrWhiteSpace(_materialProperty))
            {
                Debug.LogError("Material property name not set!");
                return false;
            }

            // if null- return null without caching
            if (mat == null)
            {
                Debug.LogError("Material is null!");
                return false;
            }

            if (!mat.HasProperty(_materialProperty))
            {
                Debug.LogError($"Material {mat.name} doesn't have property with name {_materialProperty}");
                return false;
            }
            
            // Can't test material type to ensure color as far as I know

            _validMaterials.Add(mat);
            return true;
        }

        private Color GetColor(MapEffectState state)
        {
            switch (state)
            {
                case MapEffectState.Unavailable: return _unavailable;
                case MapEffectState.Normal: return _available;
                case MapEffectState.Highlighted: return _highlighted;
                case MapEffectState.HasCameraFocus: return _currentView;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}