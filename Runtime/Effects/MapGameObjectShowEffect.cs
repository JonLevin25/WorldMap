using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace WorldMap.Effects
{
    public class MapGameObjectShowEffect : MapSelectionEffectBase
    {
        [ReorderableList]
        [SerializeField] private GameObject[] _gameObjects;
        
        [ReorderableList]
        [SerializeField] private MapEffectState[] _shownStates;

        private IEnumerable<MapEffectState> ShownStates => _shownStates?.Distinct();


        public override void Bind(MapEffectState state)
        {
            var goState = ShownStates.Contains(state);
            foreach (var go in _gameObjects)
            {
                go.SetActive(goState);
            }
        }
    }
}