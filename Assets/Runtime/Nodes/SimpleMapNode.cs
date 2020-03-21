using UnityEngine;

namespace GalaxyMap.Nodes
{
    /// <summary>
    /// The simplest implementation of a MonoBehaviour IMapNode <br />
    /// Completion can be set in inspector, or modified at runtime
    /// </summary>
    public class SimpleMapNode : MapNodeBase
    {
        [SerializeField] private bool _completed;

        public override bool Completed => _completed;

        // Can't add setter without adding in base - implemented in method
        public void SetCompleted(bool completed)
        {
            _completed = completed;
        }
    }
}