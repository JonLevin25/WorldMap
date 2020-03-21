using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WorldMap.Nodes
{
    // TODO: Unit tests
    public class MapNodeList : IEnumerable<IMapNode>
    {
        private readonly LinkedList<IMapNode> _linkedList;
        private LinkedListNode<IMapNode> _currLinkedNode;
        
        public IMapNode Current => _currLinkedNode?.Value;
        public IMapNode First => _linkedList.First?.Value;
        public IMapNode Last => _linkedList.Last?.Value;
        
        
        public IEnumerable<IMapNode> All => _linkedList;
        
        /// <summary>
        /// Should selection wrap around from last to first, and from first to last?
        /// </summary>
        public bool WrapAround { get; set; }

        public MapNodeList(IEnumerable<IMapNode> nodes, bool wrapAround)
        {
            nodes = nodes.Where(node => node != null);
            
            if (!nodes.Any()) throw new Exception("No Nodes found!");
            // TODO: validate no duplicate references (could cause infinite loops)
            
            _linkedList = new LinkedList<IMapNode>(nodes);
            WrapAround = wrapAround;
        }

        #region Public: IEnumerable<IMapNode> methods
        
        public IEnumerator<IMapNode> GetEnumerator() => All.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        #endregion

        #region Public: Selection Methods
        
        public void SelectNone() => _currLinkedNode = null;

        public IMapNode SelectFirstAvailable()
        {
            var firstAvailNode = FirstAvailableOrNull(AllLinkedNodes());
            SelectIfNotNull(firstAvailNode);
            return Current;
        }

        public IMapNode SelectLastAvailable()
        {
            var reversedNodes = AllLinkedNodes().Reverse();
            var lastAvailNode = FirstAvailableOrNull(reversedNodes);
            SelectIfNotNull(lastAvailNode);
            return Current;
        }

        public IMapNode SelectNextAvailable()
        {
            if (Current == null) return SelectFirstAvailable();
            
            // Go over nodes until end, then back from start
            // Find the first one that's available, and select it.
            var nextNodes = FromNodeToEnd(_currLinkedNode).Skip(1);
            var wrapAroundNodes = FromStartToNode(_currLinkedNode);

            var nodes = WrapAround ? nextNodes.Concat(wrapAroundNodes) : nextNodes;
            var firstAvailable = FirstAvailableOrNull(nodes);

            SelectIfNotNull(firstAvailable);
            return Current;
        }

        public IMapNode SelectPrevAvailable()
        {
            if (Current == null)
            {
                return WrapAround 
                    ? SelectLastAvailable() 
                    : SelectFirstAvailable();
            }
            
            // Go over nodes in reverse order until start, then back from end again
            // Find the first one that's available, and select it.
            var prevNodes = FromNodeBackToStart(_currLinkedNode).Skip(1);
            var wrapAroundNodes = FromEndBackToNode(_currLinkedNode);

            var nodes = WrapAround ? prevNodes.Concat(wrapAroundNodes) : prevNodes;
            var firstAvailable = FirstAvailableOrNull(nodes);

            SelectIfNotNull(firstAvailable);
            return Current;
        }

        /// <summary>
        /// Try to select a node.
        /// Return false if the node was null, not available, or not found.
        /// Return True otherwise.
        /// </summary>
        /// <param name="node">The node to select</param>
        public bool SelectIfAvailable(IMapNode node)
        {
            if (node == null) return false;
            if (!node.Available) return false;
            
            var foundNode = AllLinkedNodes().FirstOrDefault(linkedNode => linkedNode.Value == node);
            if (foundNode == null)
            {
                Debug.LogError("Node not found!");
                return false;
            }

            Select(foundNode);
            return true;
        }
        
        #endregion
        
        #region Public: Iterators

        public IEnumerable<IMapNode> IterFrom(IMapNode node)
        {
            // Validate Input
            if (node == null)
            {
                throw new ArgumentNullException($"Cannot {nameof(IterFrom)} a null node!");
            }
            
            var linkedNode = _linkedList.Find(node);
            if (linkedNode == null)
            {
                Debug.LogError($"{nameof(IterFrom)}: Node not found!");
            }

            // Wrap private iterator
            return FromNodeToEnd(linkedNode)
                .Select(linkedNode2 => linkedNode2.Value);
        }
        
        #endregion

        #region Private: Iterators
        
        private IEnumerable<LinkedListNode<IMapNode>> AllLinkedNodes()
        {
            var linkedNode = _linkedList.First;
            while (linkedNode != null)
            {
                yield return linkedNode;
                linkedNode = linkedNode.Next;
            }
        }

        private static IEnumerable<LinkedListNode<IMapNode>> FromNodeToEnd(LinkedListNode<IMapNode> linkedNode)
        {
            while (linkedNode != null)
            {
                yield return linkedNode;
                linkedNode = linkedNode.Next;
            }
        }
        
        private static IEnumerable<LinkedListNode<IMapNode>> FromNodeBackToStart(LinkedListNode<IMapNode> linkedNode)
        {
            while (linkedNode != null)
            {
                yield return linkedNode;
                linkedNode = linkedNode.Previous;
            }
        }
        
        // "Reverse" collection
        private static IEnumerable<LinkedListNode<IMapNode>> FromEndBackToNode(LinkedListNode<IMapNode> node)
            => FromNodeToEnd(node).Reverse();

        private static IEnumerable<LinkedListNode<IMapNode>> FromStartToNode(LinkedListNode<IMapNode> node)
            => FromNodeBackToStart(node).Reverse();

        #endregion
        
        #region Private: Helpers
        
        
        private static LinkedListNode<IMapNode> FirstAvailableOrNull(IEnumerable<LinkedListNode<IMapNode>> collection)
        {
            return collection.FirstOrDefault(node => node.Value.Available);
        }
        
        private void SelectIfNotNull(LinkedListNode<IMapNode> node)
        {
            if (node == null) return;
            Select(node);
        }

        private void Select(LinkedListNode<IMapNode> node)
        {
            _currLinkedNode = node;
        }

        #endregion
    }
}