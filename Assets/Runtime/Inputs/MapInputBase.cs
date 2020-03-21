using System;
using UnityEngine;

namespace GalaxyMap.Inputs
{
    public abstract class MapInputBase : MonoBehaviour
    {
        public abstract event Action<MapInputPayload> OnInputUpdate;
    }
}