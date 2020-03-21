using System.Linq;
using GalaxyMap;
using GalaxyMap.Nodes;
using NaughtyAttributes;
using UnityEngine;

namespace GalaxyMap.Effects
{
// TODO: extract subscription logic to GalaxyNodeObserver or similar
    public class GalaxyMapConnector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LineRenderer _activeLine;
        [SerializeField] private LineRenderer _inactiveLine;
        [SerializeField] private Transform _start;
        [SerializeField] private Transform _end;

        [Header("Look & Feel")]
        [SerializeField] private int _lineVertices = 20;

        [Header("State")]
        [SerializeField] private bool _connectionActive;

        private IGalaxyNode _node;

        public bool ConnectionActive
        {
            get => _connectionActive;
            set
            {
                _connectionActive = value;
                Bind();
            }
        }

        public Vector3 StartPos
        {
            get => _start.position;
            set
            {
                _start.position = value;
                Bind();
            }
        }

        public Vector3 EndPos
        {
            get => _end.position;
            set
            {
                _end.position = value;
                Bind();
            }
        }

        public void SetPositions(Vector3 start, Vector3 end)
        {
            _start.position = start;
            _end.position = end;

            Bind();
        }

        private void Awake() => Subscribe(_node);

        private void OnDestroy() => Unsubscribe(_node);

        public void SetSelectable(IGalaxyNode newNode)
        {
            // Unsubscribe from old node if relevant
            if (newNode != _node)
            {
                Unsubscribe(_node);
            }

            _node = newNode;
            Subscribe(_node);
            Bind(_node);
        }

        private void Subscribe(IGalaxyNode node)
        {
            if (node == null) return;
            node.OnStateChanged += Bind;
        }

        private void Unsubscribe(IGalaxyNode node)
        {
            if (node == null) return;
            node.OnStateChanged -= Bind;
        }

        private void Bind(IGalaxyNode node)
        {
            if (node == null) return;

            var active = node.Available || node.Focused;
            _connectionActive = active;
            Bind();
        }

        [Button("Bind")]
        private void Bind()
        {
            var trans = transform;
            trans.position = Vector3.zero;
            trans.rotation = Quaternion.identity;
            trans.localScale = Vector3.one;

            Vector3 Lerp(int i) => Vector3.Lerp(StartPos, EndPos, (float) i / _lineVertices);

            var linePoints = Enumerable.Range(0, _lineVertices)
                .Select(Lerp)
                .ToArray();

            _activeLine.SetPositions(linePoints);
            _inactiveLine.SetPositions(linePoints);

            _activeLine.gameObject.SetActive(ConnectionActive);
            _inactiveLine.gameObject.SetActive(!ConnectionActive);
        }
    }
}
