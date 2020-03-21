using GalaxyMap.Nodes;
using UnityEngine;

namespace GalaxyMap.Inputs
{
    public class LinearMapInputDelegate : MapInputDelegateBase
    {
        // NOTE: Whenever setting this, make sure base._nodeManager is set as well!
        // If it isn't, then whenever base._nodeManager is used
        // (such as from (non-overridden) inherited method - it will throw NullRef
        private new ILinearMapNodeManager _mapNodeManager;

        public override void Init(IMapController mapController, INodeManager nodeManager)
        {
            base.Init(mapController, nodeManager);
            SetNodeManager(nodeManager);
        }

        protected void SelectNextNode() => _mapNodeManager.SelectNext();
        protected void SelectPrevNode() => _mapNodeManager.SelectPrev();


        public override void OnNodeClicked(IMapNode node)
        {
            if (node == null) return;
            if (_mapController.SelectedNode == node)
            {
                _mapController.ZoomIn(node);
            }
            else
            {
                _mapController.TrySetFocused(node);
            }
        }

        protected override void OnInput(MapInputPayload payload)
        {
            OnNavigationInput(payload.Selection);
            OnCameraInput(payload.Camera);
            
            if (payload.SubmitButton) OnSubmit();
            if (payload.CancelButton) OnCancel();
        }

        protected virtual void OnNavigationInput(Vector2 navDirection)
        {
            // TODO: can I use Unity UI navigation here (Selectables) for better navigation? (i.e. "up" will move to next planet above, whether its the "next" or previous)
            if (_mapController.ZoomedNode != null) return; // Disable navigation when zoomed in

            // Prefer x axis for navigation, fallback on y
            if (navDirection.x > 0) SelectNextNode();
            else if (navDirection.x < 0) SelectPrevNode();
            
            else if (navDirection.y > 0) SelectNextNode();
            else if (navDirection.y < 0) SelectPrevNode();
        }
        
        protected virtual void OnCameraInput(Vector2 input)
        {
            _mapController.MoveCamera(input);
        }

        protected virtual void OnSubmit()
        {
            if (_mapController.ZoomedNode != _mapController.SelectedNode)
            {
                _mapController.ZoomIn(_mapController.SelectedNode);
            }
        }

        protected virtual void OnCancel() => NavigateBack(_mapController.ZoomedNode);
        
        protected virtual void NavigateBack(IMapNode zoomedNode)
        {
            if (zoomedNode == null) return;
            _mapController.ZoomOut();
        }

        private void SetNodeManager(INodeManager nodeManager)
        {
            if (nodeManager == null)
            {
                Debug.Log("Can't set null NodeManager!");
                return;
            }
            
            if (nodeManager is ILinearMapNodeManager linearNodeManager)
            {
                _mapNodeManager = linearNodeManager;
                
                // Make sure base (hidden) nodeManager is set, since it may be used in some scenarios
                base._nodeManager = nodeManager;
                return;
            }

            Debug.LogError($"{GetType().Name} Expects a ~Linear~ NodeManager! " +
                           $"NodeManager was of type: {nodeManager.GetType()}");
        }
    }
}