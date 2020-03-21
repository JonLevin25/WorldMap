using GalaxyMap.Nodes;
using UnityEngine;

namespace GalaxyMap.Inputs
{
    public abstract class MapInputDelegateBase : MonoBehaviour
    {
        [SerializeField] protected MapInputBase _input;
        
        protected IMapController _mapController;
        protected INodeManager _nodeManager;

        public virtual void Init(IMapController mapController, INodeManager nodeManager)
        {
            _mapController = mapController;
            _nodeManager = nodeManager;
            
            _mapController.OnNodeClicked += OnNodeClicked;
            _input.OnInputUpdate += OnInput;
        }

        /// <summary>
        /// When World Map state changes, this will be called. <br />
        /// If you have custom data binding logic you can override this 
        /// </summary>
        public virtual void Bind() { }

        public abstract void OnNodeClicked(IMapNode node);

        protected abstract void OnInput(MapInputPayload payload);

        protected void OnDestroy()
        {
            _mapController.OnNodeClicked -= OnNodeClicked;
        }
    }
}
