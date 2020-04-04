using UnityEngine;

namespace WorldMap.Effects
{
    public class MapMaterialSelectionEffect : MapSelectionEffectBase
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private MaterialKeywordEnabler _materialKeywords;
        [SerializeField] private MaterialEffectColorBlock[] _materialColors;
        
#if UNITY_EDITOR
        private void Start()
        {
            // Fix for emission not working in editor until playing with emission in material manually 
            foreach (var keyword in _materialKeywords.GetKeywords())
            {
                _renderer.material.EnableKeyword(keyword);
            }
        }
#endif
        
        public override void Bind(MapEffectState state)
        {
            foreach (var colorBlock in _materialColors)
            {
                colorBlock.Bind(_renderer.material, state);
            }
        }
    }
}