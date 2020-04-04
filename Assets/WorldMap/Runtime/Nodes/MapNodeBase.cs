using System;
using Cinemachine;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using WorldMap.Utils;

namespace WorldMap.Nodes
{
    public abstract class MapNodeBase : MonoBehaviour, IMapNode
    {
        [SerializeField] private CinemachineVirtualCameraBase _viewCamera;
        [SerializeField] private Collider _collider;

        public event Action<IMapNode> OnClicked;
        public event Action<IMapNode> OnStateChanged;

        private bool _available;
        private bool _selected;
        private bool _mouseOver;

        public abstract bool Completed { get; }
        
        public CinemachineVirtualCameraBase ViewCamera => _viewCamera;
        public Vector3 Position => transform.position;
        
        [ShowNativeProperty]
        public bool Available
        {
            get => _available;
            set
            {
                if (_available == value) return;
                _available = value;

                OnStateChanged?.Invoke(this);
            }
        }


        [ShowNativeProperty]
        public bool Focused
        {
            get => _selected;
            set
            {
                if (_selected == value) return;
                _selected = value;

                OnStateChanged?.Invoke(this);
            }
        }

        [ShowNativeProperty]
        public bool IsMouseOver
        {
            get => _mouseOver;
            protected set
            {
                if (_mouseOver == value) return;
                _mouseOver = value;

                OnStateChanged?.Invoke(this);
            }
        }


        protected virtual void Awake()
        {
            ConfigMouseEventForwarding(_collider.gameObject, gameObject);
        }

        private void OnMouseEnter() => IsMouseOver = true;

        private void OnMouseExit() => IsMouseOver = false;

        private void OnMouseUpAsButton() => OnClicked?.Invoke(this);

        // So derived classes can invoke OnStateChanged
        protected void OnStateChangedInternal() => OnStateChanged?.Invoke(this);

        private static void ConfigMouseEventForwarding(GameObject forwardingGO, GameObject receivingGameObject)
        {
            var mouseMessageForwarder = forwardingGO.AddComponent<MonoMouseMessageForwarder>();
            mouseMessageForwarder.Messages = new[]
            {
                MonoMouseMessageForwarder.Message.OnMouseEnter,
                MonoMouseMessageForwarder.Message.OnMouseExit,
                MonoMouseMessageForwarder.Message.OnMouseUpAsButton
            };

            mouseMessageForwarder.Target = receivingGameObject;
        }

#if UNITY_EDITOR
        
        // [Button("Find & Set Collider in children")] // Didn't work - serialization issues with prefabs. Should work once updated NaughtyAttributes
        [ContextMenu("Find & Set Collider in children")]
        protected void SetColliderFromChildren()
        {
            const string logPrefix = nameof(SetColliderFromChildren) + ":";
            
            Assert(_collider == null, $"{logPrefix} Collider is already set! will not replace");
            
            var colliders = GetComponentsInChildren<Collider>();
            
            var noCollidersFoundMsg = $"{logPrefix} No colliders found!";
            Assert(colliders != null, noCollidersFoundMsg);
            Assert(colliders.Length != 0, noCollidersFoundMsg);
            Assert(colliders.Length == 1, $"{logPrefix} more than one collider found!");

            var collider = colliders[0];

            Debug.Log($"{logPrefix} Successfully set collider from children!");
            _collider = collider;
            
        }

        private static void Assert(bool condition, string msg)
        {
            if (condition) return;
            // Debug.LogAssertion(msg);
            throw new AssertionException(msg, "");
        }
#endif
    }
    
}