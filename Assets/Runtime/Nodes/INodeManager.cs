using System;
using System.Collections.Generic;

namespace GalaxyMap.Nodes
{
    public interface INodeManager
    {
        event Action<IGalaxyNode> OnNodeSelected;
        event Action<IGalaxyNode> OnNodeClicked;
        
        IEnumerable<IGalaxyNode> AllNodes { get; }

        IGalaxyNode Selected { get; }
        
        bool SelectIfAvailable(IGalaxyNode node);
        
        /// <summary>
        /// Makes sure all nodes are correctly labeled 'available' or 'unavailable'
        /// </summary>
        void UpdateNodeAvailability();
    }
}