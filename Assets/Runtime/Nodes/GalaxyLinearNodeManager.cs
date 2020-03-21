using System;
using System.Collections.Generic;
using System.Linq;
using GalaxyMap.Effects;
using GalaxyMap.Utils;
using NaughtyAttributes;
using UnityEngine;

namespace GalaxyMap.Nodes
{
    public class GalaxyLinearNodeManager : MonoBehaviour, ILinearNodeManager
    {
        [SerializeField] private GalaxyMapConnector _connectorPrefab;
        
        [ReorderableList]
        [SerializeField] private GalaxyNodeBase[] _nodes;

        [Header("Selection")]
        [Tooltip("Whether last node will wrap around to first, and vice-versa")]
        [SerializeField] private bool _wrapAround;

        public event Action<IGalaxyNode> OnNodeSelected;
        public event Action<IGalaxyNode> OnNodeClicked;

        private GalaxyNodeList _nodeList;
        private PrefabPool<GalaxyMapConnector> _connectorPool;

        public IGalaxyNode Selected => _nodeList?.Current;
        public IEnumerable<IGalaxyNode> AllNodes => _nodeList?.All;

        public IGalaxyNode FirstNode => _nodeList.First;
        public IGalaxyNode LastNode => _nodeList.Last;


        private void Awake()
        {
            // Initialize selectables, availability
            _nodeList = new GalaxyNodeList(_nodes, _wrapAround);
            BindAvailability(_nodeList);

            if (_connectorPrefab != null)
            {
                _connectorPool = new PrefabPool<GalaxyMapConnector>(_connectorPrefab, "ConnectorPool");
                BindConnectors(_connectorPool, _nodeList);
            }

            // Initialize selection
            _nodeList.SelectFirstAvailable();
            OnSelectionChanged();
            
            foreach (var node in _nodes)
            {
                OnNodeAdded(node);
            }
        }

        private void OnValidate()
        {
            if (_nodeList != null) _nodeList.WrapAround = _wrapAround;
            
        }

        private void OnDestroy()
        {
            foreach (var node in _nodes)
            {
                OnNodeRemoved(node);
            }
        }

        /// <summary>
        /// Try to select a node<br />
        /// Return false if the node was null, not available, or not found<br />
        /// Return True otherwise
        /// </summary>
        /// <param name="node">The node to select</param>
        public bool SelectIfAvailable(IGalaxyNode node)
        {
            if (_nodeList.SelectIfAvailable(node))
            {
                OnSelectionChanged();
                return true;
            }
            
            return false;
        }

        public bool SelectNext()
        {
            var next = _nodeList.SelectNextAvailable();
            if (next != null)
            {
                OnSelectionChanged();
                return true;
            }
            
            return false;
        }

        public bool SelectPrev()
        {
            var prev = _nodeList.SelectPrevAvailable();
            if (prev != null)
            {
                OnSelectionChanged();
                return true;
            }

            return false;
        }

        public void UpdateNodeAvailability() => BindAvailability(_nodeList);

        private static void BindConnectors(PrefabPool<GalaxyMapConnector> connectorPool, IEnumerable<IGalaxyNode> selectables)
        {
            if (connectorPool == null || selectables == null) return;
            
            connectorPool.RecycleAll();

            IGalaxyNode last = null;
            foreach (var selectable in selectables)
            {
                if (last == null)
                {
                    last = selectable;
                    continue;
                }

                var connector = connectorPool.Get();
                connector.SetPositions(last.Position, selectable.Position);
                connector.SetSelectable(selectable);

                last = selectable;
            }
        }

        private void OnSelectionChanged()
        {
            bool IsSelected(GalaxyNodeBase node) => ReferenceEquals(node, _nodeList.Current);
            
            OnNodeSelected?.Invoke(Selected);
            foreach (var node in _nodes)
            {
                node.Focused = IsSelected(node);
            }
        }

        /// <summary>
        /// Go over nodes and set their available status
        /// </summary>
        /// <param name="nodes">The linear collection of nodes to bind</param>
        private static void BindAvailability(IEnumerable<IGalaxyNode> nodes)
        {
            if (nodes == null) return;
            
            // Nodes are available only if all previous nodes completed
            var allCompleted = true;
            foreach (var node in nodes)
            {
                node.Available = allCompleted;
                allCompleted = allCompleted && node.Completed;
            }
        }

        private void OnNodeStateChanged(IGalaxyNode updatedNode)
        {
            // In case completion state changed, update the availability of the nodes after it 
            // TODO: if performance becomes an issue - can be optimized by refactoring to stateful iteration only on relevant nodes
            BindAvailability(_nodeList);
        }

        private void ForwardNodeClick(IGalaxyNode node) => OnNodeClicked?.Invoke(node);

        private void OnNodeAdded(IGalaxyNode node)
        {
            node.OnClicked += ForwardNodeClick;
            node.OnStateChanged += OnNodeStateChanged;
        }

        private void OnNodeRemoved(IGalaxyNode node)
        {
            node.OnClicked -= ForwardNodeClick;
            node.OnStateChanged -= OnNodeStateChanged;
        }
    }
}