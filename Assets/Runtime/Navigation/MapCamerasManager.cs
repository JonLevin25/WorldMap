using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using GalaxyMap.Nodes;
using GalaxyMap.Utils;
using UnityEngine;

namespace GalaxyMap.Navigation
{
    public class MapCamerasManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCameraBase _mainCamera;
//        [SerializeField] private CinemachineVirtualCameraBase _closeupCamera;
        [SerializeField] private MapGhostPointer _ghostPointer;
        [SerializeField] private BoundsHolder _fullViewBounds;
        
        private IEnumerable<CinemachineVirtualCameraBase> _viewCameras = Enumerable.Empty<CinemachineVirtualCameraBase>();

        private IEnumerable<CinemachineVirtualCameraBase> AllCams 
            => _viewCameras.Prepend(_mainCamera).Where(cam => cam != null);

        private const int LowPriority = 0;
        private const int HighPriority = 100;

        private void Awake() => ZoomOut();

        public void SetViewCameras(IEnumerable<CinemachineVirtualCameraBase> viewCameras)
        {
            _viewCameras = viewCameras;
        }

        public void Move(Vector3 movement)
        {
            _ghostPointer.Move(movement);
        }

        public void JumpTo(Vector3 position) => _ghostPointer.JumpTo(position);

        public void ZoomIn(IMapNode node)
        {
            // TODO: have root node for "whole map" so null isn't needed?
            if (node == null) ZoomOut();
            else
            {
                if (node.ViewCamera != null) SetAsActiveCamera(node.ViewCamera);
                else JumpTo(node.Position);
            }
        }

        private void ZoomOut()
        {
            _ghostPointer.PointerBounds = _fullViewBounds;
            _ghostPointer.PointerSpeed = MapGhostPointer.PointerSpeedType.Fast;

            SetAsActiveCamera(_mainCamera);
        }

        private void SetAsActiveCamera(CinemachineVirtualCameraBase activeCam)
        {
            if (activeCam == null) return;
            
            foreach (var cam in AllCams)
                cam.Priority = LowPriority;

            activeCam.Priority = HighPriority;
        }
    }
}