using System;
using Cinemachine;
using UnityEngine;

namespace GalaxyMap.Nodes
{
    public interface IGalaxyNode
    {
        event Action<IGalaxyNode> OnStateChanged;
        event Action<IGalaxyNode> OnClicked;

        CinemachineVirtualCameraBase ViewCamera { get; }
        Vector3 Position { get; }

        bool Completed { get; }
        bool Available { get; set; }
        bool Focused { get; }
        bool IsMouseOver { get; }
    }
}