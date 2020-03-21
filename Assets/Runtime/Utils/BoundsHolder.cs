using NaughtyAttributes;
using UnityEngine;

namespace WorldMap.Utils
{
    public class BoundsHolder : MonoBehaviour
    {
        [SerializeField] private Vector3 _center;
        [SerializeField] private Vector3 _size = Vector3.one;

        [InfoBox(BoundsRotationWarningMessage, InfoBoxType.Warning, nameof(IsRotated))]
        [SerializeField] private Color _gizmoColor = Color.magenta;
        
        private const string BoundsRotationWarningMessage = "Bounds will not reflect rotation! only size & position will be presereved.";

        public Bounds GlobalBounds => GetBounds(transform, _center, _size);
        
        // For Custom inspector
        private bool IsRotated() => transform.rotation != Quaternion.identity; 


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _gizmoColor;
            var bounds = GlobalBounds;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            // TODO: handles?
        }
#endif

        private static Bounds GetBounds(Transform trans, Vector3 localCenter, Vector3 localSize)
        {
            var center = trans.TransformPoint(localCenter);

            var size = localSize;
            size.x /= trans.lossyScale.x;
            size.y /= trans.lossyScale.y;
            size.z /= trans.lossyScale.z;

            return new Bounds(center, size);
        }
    }
}
