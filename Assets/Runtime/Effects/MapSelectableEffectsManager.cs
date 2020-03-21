using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaxyMap.DependencyInjection;
using GalaxyMap.Nodes;
using NaughtyAttributes;
using UnityEngine;

namespace GalaxyMap.Effects
{
    public class MapSelectableEffectsManager : MonoBehaviour
    {
        [SerializeField] private MapNodeBase node;
        
        [Space]
        [InfoBox(GetComponentWarningMessage, InfoBoxType.Warning, nameof(_searchChildrenAtRuntime))]
        [SerializeField] private bool _searchChildrenAtRuntime;
        [SerializeField] private bool _setEffectRefernces = true;
        
        [ShowIf(nameof(_setEffectRefernces))]
        [ReorderableList]
        [SerializeField] private MapSelectionEffectBase[] _effects;
        
        private const string GetComponentWarningMessage = "GetComponentInChildren will run each time selection is changed!" +
                                                          "\nThis is inefficient, and only recommended if you don't have access to the references at edit time";

        private IMapController _mapController;
        private Task _initTask; // Initialization in Awake() is async, await this to make sure it finishes

        private IEnumerable<IMapSelectionEffect> Effects
        {
            get
            {
                var serializedEffects = _setEffectRefernces 
                    ? _effects.Where(e => e != null) 
                    : Enumerable.Empty<IMapSelectionEffect>();
                
                var childEffects = _searchChildrenAtRuntime 
                    ? GetComponentsInChildren<IMapSelectionEffect>()
                    : Enumerable.Empty<IMapSelectionEffect>();
                
                foreach (var effect in serializedEffects.Concat(childEffects))
                {
                    yield return effect;
                }
            }
        }

        private async void Awake()
        {
            var initTaskCompletion = new TaskCompletionSource<bool>();
            _initTask = initTaskCompletion.Task;
            
            if (node == null)
            {
                Debug.LogError($"{nameof(MapNodeBase)} not set in {nameof(MapSelectableEffectsManager)} {name}!");
                initTaskCompletion.SetResult(true);
                return;
            }
            
            node.OnStateChanged += OnNodeStateChanged;

            // Get the WorldMap, async in case it's Awake runs after this, or it's loaded at runtime
            const float mapControllerTimeout = 0.2f; // Arbitrary short time, just to give MapController a chance to load 
            _mapController = await MapDependencyContainer.ResolveAsync<IMapController>(mapControllerTimeout);
            if (_mapController == null)
            {
                initTaskCompletion.SetResult(false);
                return;
            }

            _mapController.OnZoomed += OnZoomedIn;
            _mapController.OnNodeClicked += OnNodeClicked;
            initTaskCompletion.SetResult(true);
        }

        private void Start() => OnNodeStateChanged(node);

        private void OnDestroy()
        {
            // Unsubscribe
            node.OnStateChanged -= OnNodeStateChanged;
            
            _mapController.OnZoomed -= OnZoomedIn;
            _mapController.OnNodeClicked -= OnNodeClicked;
        }

        private void OnZoomedIn(IMapNode zoomedNode) => Bind();

        private void OnNodeClicked(IMapNode clickedNode) => Bind();

        private void OnNodeStateChanged(IMapNode node) => Bind();

        private async void Bind()
        {
            await _initTask;
            var state = GetState(_mapController, node);
            foreach (var effect in Effects)
            {
                effect.Bind(state);
            }
        }
        
        private static MapEffectState GetState(IMapController mapController, IMapNode node)
        {
            if (!node.Available) return MapEffectState.Unavailable;
            if (IsZoomedInOn(mapController, node)) return MapEffectState.HasCameraFocus;
            if (IsHighlighted(node)) return MapEffectState.Highlighted;
            
            return MapEffectState.Normal;
        }

        private static bool IsZoomedInOn(IMapController controller, IMapNode node) => controller.ZoomedNode == node;

        private static bool IsHighlighted(IMapNode node) => node.Focused || node.IsMouseOver;
    }
}