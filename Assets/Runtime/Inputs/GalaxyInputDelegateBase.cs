using GalaxyMap.Nodes;
using UnityEngine;

namespace GalaxyMap.Inputs
{
    public abstract class GalaxyInputDelegateBase : MonoBehaviour
    {
        [SerializeField] protected GalaxyInputBase _input;
        
        protected IGalaxyMapController _galaxyMap;
        protected INodeManager _nodeManager;

        public virtual void Init(IGalaxyMapController mapController, INodeManager nodeManager)
        {
            _galaxyMap = mapController;
            _nodeManager = nodeManager;
            
            _galaxyMap.OnNodeClicked += OnNodeClicked;
            _input.OnInputUpdate += OnInput;
        }

        /// <summary>
        /// When Galaxy Map state changes, this will be called. <br />
        /// If you have custom data binding logic you can override this 
        /// </summary>
        public virtual void Bind() { }

        public abstract void OnNodeClicked(IGalaxyNode node);

        protected abstract void OnInput(GalaxyInputPayload payload);

        protected void OnDestroy()
        {
            _galaxyMap.OnNodeClicked -= OnNodeClicked;
        }
    }
}
