using UnityEngine;

namespace GalaxyMap.Effects
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