using System;
using System.Collections.Generic;

namespace WorldMap.Nodes
{
    public interface INodeManager
    {
        event Action<IMapNode> OnNodeSelected;
        event Action<IMapNode> OnNodeClicked;
        
        IEnumerable<IMapNode> AllNodes { get; }

        IMapNode Selected { get; }
        
        bool SelectIfAvailable(IMapNode node);
        
        /// <summary>
        /// Makes sure all nodes are correctly labeled 'available' or 'unavailable'
        /// </summary>
        void UpdateNodeAvailability();
    }
}