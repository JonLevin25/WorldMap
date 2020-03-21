using UnityEngine;

namespace WorldMap.Effects
{
    public class MapMaterialSelectionEffect : MapSelectionEffectBase
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private MaterialEffectColorBlock[] _materialColors;
        
        public override void Bind(MapEffectState state)
        {
            foreach (var colorBlock in _materialColors)
            {
                colorBlock.Bind(_renderer.material, state);
            }
        }
    }
}