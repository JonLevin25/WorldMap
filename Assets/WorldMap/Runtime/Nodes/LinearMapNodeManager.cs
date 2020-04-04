using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using WorldMap.Effects;
using WorldMap.Utils;

namespace WorldMap.Nodes
{
    public class LinearMapNodeManager : MonoBehaviour, ILinearMapNodeManager
    {
        [SerializeField] private MapNodesConnector nodesConnectorPrefab;
        
        [ReorderableList]
        [SerializeField] private MapNodeBase[] _nodes;

        [Header("Selection")]
        [Tooltip("Whether last node will wrap around to first, and vice-versa")]
        [SerializeField] private bool _wrapAround;

        public event Action<IMapNode> OnNodeSelected;
        public event Action<IMapNode> OnNodeClicked;

        private MapNodeList _nodeList;
        private PrefabPool<MapNodesConnector> _connectorPool;

        public IMapNode Selected => _nodeList?.Current;
        public IEnumerable<IMapNode> AllNodes => _nodeList?.All;

        public IMapNode FirstNode => _nodeList.First;
        public IMapNode LastNode => _nodeList.Last;


        private void Awake()
        {
            // Initialize selectables, availability
            _nodeList = new MapNodeList(_nodes, _wrapAround);
            BindAvailability(_nodeList);

            if (nodesConnectorPrefab != null)
            {
                _connectorPool = new PrefabPool<MapNodesConnector>(nodesConnectorPrefab, "ConnectorPool");
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
        public bool SelectIfAvailable(IMapNode node)
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

        private static void BindConnectors(PrefabPool<MapNodesConnector> connectorPool, IEnumerable<IMapNode> selectables)
        {
            if (connectorPool == null || selectables == null) return;
            
            connectorPool.RecycleAll();

            IMapNode last = null;
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
            bool IsSelected(MapNodeBase node) => ReferenceEquals(node, _nodeList.Current);
            
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
        private static void BindAvailability(IEnumerable<IMapNode> nodes)
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

        private void OnNodeStateChanged(IMapNode updatedNode)
        {
            // In case completion state changed, update the availability of the nodes after it 
            // TODO: if performance becomes an issue - can be optimized by refactoring to stateful iteration only on relevant nodes
            BindAvailability(_nodeList);
        }

        private void ForwardNodeClick(IMapNode node) => OnNodeClicked?.Invoke(node);

        private void OnNodeAdded(IMapNode node)
        {
            node.OnClicked += ForwardNodeClick;
            node.OnStateChanged += OnNodeStateChanged;
        }

        private void OnNodeRemoved(IMapNode node)
        {
            node.OnClicked -= ForwardNodeClick;
            node.OnStateChanged -= OnNodeStateChanged;
        }

#if UNITY_EDITOR

        [ContextMenu("Find Nodes In Children")]
        public void FindNodesFromChildren()
        {
            if (_nodes.Length > 0)
            {
                var confirm = UnityEditor.EditorUtility.DisplayDialog("Confirm replace",
                    $"{GetType().Name} already has {_nodes.Length} nodes! Replace them?", "Yes, Replace", "No");
                if (!confirm)
                {
                    Debug.Log("Did not replace");
                    return;
                }
            }

            var nodes = GetComponentsInChildren<MapNodeBase>();
            _nodes = nodes;
            Debug.Log($"Replaced nodes! ({nodes.Length} found)");
        }

#endif
    }
}