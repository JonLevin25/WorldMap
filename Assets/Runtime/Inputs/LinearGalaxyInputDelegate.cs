using GalaxyMap.Nodes;
using UnityEngine;

namespace GalaxyMap.Inputs
{
    public class LinearGalaxyInputDelegate : GalaxyInputDelegateBase
    {
        // NOTE: Whenever setting this, make sure base._nodeManager is set as well!
        // If it isn't, then whenever base._nodeManager is used
        // (such as from (non-overridden) inherited method - it will throw NullRef
        private new ILinearNodeManager _nodeManager;

        public override void Init(IGalaxyMapController mapController, INodeManager nodeManager)
        {
            base.Init(mapController, nodeManager);
            SetNodeManager(nodeManager);
        }

        protected void SelectNextNode() => _nodeManager.SelectNext();
        protected void SelectPrevNode() => _nodeManager.SelectPrev();


        public override void OnNodeClicked(IGalaxyNode node)
        {
            if (node == null) return;
            if (_galaxyMap.SelectedNode == node)
            {
                _galaxyMap.ZoomIn(node);
            }
            else
            {
                _galaxyMap.TrySetFocused(node);
            }
        }

        protected override void OnInput(GalaxyInputPayload payload)
        {
            OnNavigationInput(payload.Selection);
            OnCameraInput(payload.Camera);
            
            if (payload.SubmitButton) OnSubmit();
            if (payload.CancelButton) OnCancel();
        }

        protected virtual void OnNavigationInput(Vector2 navDirection)
        {
            // TODO: can I use Unity UI navigation here (Selectables) for better navigation? (i.e. "up" will move to next planet above, whether its the "next" or previous)
            if (_galaxyMap.ZoomedNode != null) return; // Disable navigation when zoomed in

            // Prefer x axis for navigation, fallback on y
            if (navDirection.x > 0) SelectNextNode();
            else if (navDirection.x < 0) SelectPrevNode();
            
            else if (navDirection.y > 0) SelectNextNode();
            else if (navDirection.y < 0) SelectPrevNode();
        }
        
        protected virtual void OnCameraInput(Vector2 input)
        {
            _galaxyMap.MoveCamera(input);
        }

        protected virtual void OnSubmit()
        {
            if (_galaxyMap.ZoomedNode != _galaxyMap.SelectedNode)
            {
                _galaxyMap.ZoomIn(_galaxyMap.SelectedNode);
            }
        }

        protected virtual void OnCancel() => NavigateBack(_galaxyMap.ZoomedNode);
        
        protected virtual void NavigateBack(IGalaxyNode zoomedNode)
        {
            if (zoomedNode == null) return;
            _galaxyMap.ZoomOut();
        }

        private void SetNodeManager(INodeManager nodeManager)
        {
            if (nodeManager == null)
            {
                Debug.Log("Can't set null NodeManager!");
                return;
            }
            
            if (nodeManager is ILinearNodeManager linearNodeManager)
            {
                _nodeManager = linearNodeManager;
                
                // Make sure base (hidden) nodeManager is set, since it may be used in some scenarios
                base._nodeManager = nodeManager;
                return;
            }

            Debug.LogError($"{GetType().Name} Expects a ~Linear~ NodeManager! " +
                           $"NodeManager was of type: {nodeManager.GetType()}");
        }
    }
}