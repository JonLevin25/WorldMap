using System;
using Cinemachine;
using GalaxyMap.Utils;
using NaughtyAttributes;
using UnityEngine;

namespace GalaxyMap.Nodes
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
    }
    
}