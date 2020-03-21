using System;
using System.Linq;
using GalaxyMap.DependencyInjection;
using GalaxyMap.Inputs;
using GalaxyMap.Navigation;
using GalaxyMap.Nodes;
using UnityEngine;

namespace GalaxyMap
{
    // Interface should contain methods for input delegate to use
    public interface IGalaxyMapController
    {
        event Action<IGalaxyNode> OnNodeClicked;
        event Action<IGalaxyNode> OnZoomed;
        
        IGalaxyNode ZoomedNode { get; }
        IGalaxyNode SelectedNode { get; }
        
        /// <summary>
        /// Try to set the given node as 'focused'.<br />
        /// May fail if the node is not available, not found in NodeManager, or null.<br />
        /// Returns true if the node is now focused, false otherwise.
        /// </summary>
        /// <param name="node">The node to set as focused</param>
        bool TrySetFocused(IGalaxyNode node);
        
        /// <summary>
        /// Zoom in on the given node.<br />
        /// Will always succeed regardless of node availability.
        /// </summary>
        /// <param name="node">The node to zoom in on</param>
        void ZoomIn(IGalaxyNode node);

        /// <summary>
        /// Zoom out to the full map view.
        /// No-op if already zoomed out
        /// </summary>
        void ZoomOut();
        
        void MoveCamera(Vector2 movement);
    }

    public class GalaxyMapController : MonoBehaviour, IGalaxyMapController
    {
        [Header("References")]
        [SerializeField] private MapCamerasManager _camerasManager;

        [Header("Config")]
        [SerializeField] private GalaxyInputDelegateBase _inputDelegate;

        public event Action<IGalaxyNode> OnNodeClicked;
        public event Action<IGalaxyNode> OnZoomed;
        
        private INodeManager _nodeManager;

        public IGalaxyNode ZoomedNode { get; private set; }

        public IGalaxyNode SelectedNode => _nodeManager.Selected;

        private void Awake()
        {
            MapDependencyContainer.RegisterSingleton<IGalaxyMapController>(this);
            
            _nodeManager = GetComponentInChildren<INodeManager>();
            if (_nodeManager == null)
            {
                Debug.LogError($"NodeManager Component required for {GetType().Name}! [On GameObject {name} or it's children]");
                return;
            }
            _inputDelegate.Init(this, _nodeManager);

            _nodeManager.OnNodeClicked += OnClicked;
            _nodeManager.OnNodeSelected += OnSelected;

            Bind();
        }

        private void Start()
        {
            // GetAll Virtual cams
            var viewCameras = _nodeManager.AllNodes
                .Where(node => node != null)
                .Select(node => node.ViewCamera);

            _camerasManager.SetViewCameras(viewCameras);
        }

        private void OnDestroy()
        {
            MapDependencyContainer.UnregisterSingleton<IGalaxyMapController>(this);
            
            _nodeManager.OnNodeClicked -= OnClicked;
            _nodeManager.OnNodeSelected -= OnSelected;
        }

        #region IGalaxyMapController members

        public bool TrySetFocused(IGalaxyNode node)
        {
            if (SelectedNode == node) return true;
          
            // NodeManager should handle null as 'select none'
            if (_nodeManager.SelectIfAvailable(node))
            {
                // If actually selected a new node, OnSelected callback should have already called Bind() 
                return true;   
            }
            
            Debug.LogError("Cannot select node! Is it Available?");
            return false;
        }

        public void ZoomIn(IGalaxyNode node) => ZoomInternal(node);

        public void ZoomOut() => ZoomInternal(null);

        public void MoveCamera(Vector2 movement)
        {
            // TODO: Curr [xz] plane hardcoded. Make customizable (allow [xy], [yz] as well)
            var realMovement = new Vector3(movement[0], 0, movement[1]);
            _camerasManager.Move(realMovement);
        }

        #endregion

        private void ZoomInternal(IGalaxyNode node)
        {
            if (ZoomedNode == node) return;
            
            ZoomedNode = node;
            OnZoomed?.Invoke(ZoomedNode);
            
            Bind();
        }

        private void OnSelected(IGalaxyNode node)
        {
            if (node == null) return;
            _camerasManager.JumpTo(node.Position);
            Bind();
        }

        private void OnClicked(IGalaxyNode node) => OnNodeClicked?.Invoke(node); // InputDelegate should handle what happens on click

        private void Bind()
        {
            _camerasManager.ZoomIn(ZoomedNode);
            if (_inputDelegate) _inputDelegate.Bind();
            
            // If zooming out, try to set move camera to "focused" node
            if (ZoomedNode == null && SelectedNode != null)
            {
                _camerasManager.JumpTo(SelectedNode.Position);
            }
        }
    }
}
