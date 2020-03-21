using System;
using UnityEngine;
using WorldMap.Utils;

namespace WorldMap.Navigation
{
    public class MapGhostPointer : MonoBehaviour
    {
        public enum PointerSpeedType
        {
            Fast,
            Slow
        }
        
        [Header("Speed config")]
        [SerializeField] private float _fastSpeed;
        [SerializeField] private float _slowSpeed;

        [Space]
        [SerializeField] private MeshRenderer _debugPointer;
        [SerializeField] private bool _showDebugPointer;

        [Header("Data")]
        [SerializeField] private PointerSpeedType _pointerSpeed;
        [SerializeField] private BoundsHolder _boundsHolder;

        [Header("Constraints")]
        [SerializeField] private bool _freezeX;
        [SerializeField] private bool _freezeY = true;
        [SerializeField] private bool _freezeZ;

        public BoundsHolder PointerBounds
        {
            get => _boundsHolder;
            set
            {
                _boundsHolder = value;
                BindPosition(transform.position);
            }
        }

        public PointerSpeedType PointerSpeed
        {
            set => _pointerSpeed = value;
        }

        private float Speed
        {
            get
            {
                switch (_pointerSpeed)
                {
                    case PointerSpeedType.Fast: return _fastSpeed;
                    case PointerSpeedType.Slow: return _slowSpeed;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        private void Awake() => Bind();

        // private void Update() => BindPosition(transform.targetPos);

        private void OnValidate() => Bind();

        public void JumpTo(Vector3 targetPos) => BindPosition(targetPos);

        public void Move(Vector3 movement)
        {
            if (!enabled) return; // Disabling component == disable Movement
            
            var distance = Speed * Time.deltaTime;

            var targetPos = transform.position + (movement * distance); 
            BindPosition(targetPos);
        }


        public void ShowDebugPointer(bool show) => _debugPointer.enabled = show;

        private void Bind()
        {
            ShowDebugPointer(_showDebugPointer);
            BindPosition(transform.position);
        }

        private void BindPosition(Vector3 targetPos)
        {
            if (PointerBounds == null) return;

            var oldPos = transform.position;
            var bounds = PointerBounds.GlobalBounds;
            
            transform.position = GetBoundPosition(oldPos, targetPos, bounds, _freezeX, _freezeY, _freezeZ);
        }

        private static Vector3 GetBoundPosition(Vector3 oldPos, Vector3 targetPos, Bounds bounds, bool freezeX, bool freezeY, bool freezeZ)
        {
            // Bound transform to closest point on bounds
            var newPos = bounds.ClosestPoint(targetPos);
            
            // Apply constraints
            if (freezeX) newPos.x = oldPos.x;
            if (freezeY) newPos.y = oldPos.y;
            if (freezeZ) newPos.z = oldPos.z;
            
            return newPos;
        }
    }
}
