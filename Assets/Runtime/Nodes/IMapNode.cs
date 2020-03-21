using System;
using Cinemachine;
using UnityEngine;

namespace WorldMap.Nodes
{
    public interface IMapNode
    {
        event Action<IMapNode> OnStateChanged;
        event Action<IMapNode> OnClicked;

        CinemachineVirtualCameraBase ViewCamera { get; }
        Vector3 Position { get; }

        bool Completed { get; }
        bool Available { get; set; }
        bool Focused { get; }
        bool IsMouseOver { get; }
    }
}