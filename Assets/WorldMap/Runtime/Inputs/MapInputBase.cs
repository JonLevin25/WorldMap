using System;
using UnityEngine;

namespace WorldMap.Inputs
{
    public abstract class MapInputBase : MonoBehaviour
    {
        public abstract event Action<MapInputPayload> OnInputUpdate;
    }
}