using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;

namespace GalaxyMap.Navigation
{
    public class CinemachineBlendCallbackWrapper : MonoBehaviour
    {
        [SerializeField] private CinemachineBrain _brain;
        [SerializeField] private CinemachineVirtualCameraBase _fromCam;
        [SerializeField] private CinemachineVirtualCameraBase _toCam;

        public event Action OnTransitionStarted;
        public event Action OnTransitionEnded;

        private bool _inTransition;
        private CancellationTokenSource _cts;
        
        private void Awake()
        {
            _brain.m_CameraActivatedEvent.AddListener(OnCameraActivated);
        }
        
        private void OnDestroy()
        {
            _brain.m_CameraActivatedEvent.RemoveListener(OnCameraActivated);
        }
        
        private async void OnCameraActivated(ICinemachineCamera newCam, ICinemachineCamera oldCam)
        {
            if (_inTransition)
            {
                _cts?.Cancel();
                _inTransition = false;
            }
            
            if (_toCam != null && !ReferenceEquals(newCam, _toCam)) return;
            if (_fromCam != null && !ReferenceEquals(oldCam, _fromCam)) return;
            
            _inTransition = true;
            _cts = new CancellationTokenSource();
            
            OnTransitionStarted?.Invoke();
            

            var transitionDuration = _brain.ActiveBlend.Duration;

            var token = _cts.Token;
            await Task.Run(() => Task.Delay(TimeSpan.FromSeconds(transitionDuration)));
            if (token.IsCancellationRequested) return;
            
            OnTransitionEnded?.Invoke();
        }
    }
}