using UnityEngine;

namespace WorldMap.Effects
{
        
    public interface IMapSelectionEffect
    {
        void Bind(MapEffectState state);
    }
    
    public abstract class MapSelectionEffectBase : MonoBehaviour, IMapSelectionEffect
    {
        public abstract void Bind(MapEffectState state);
    }
}