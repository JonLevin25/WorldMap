using System;
using UnityEngine;

namespace GalaxyMap.Inputs
{
    public abstract class GalaxyInputBase : MonoBehaviour
    {
        public abstract event Action<GalaxyInputPayload> OnInputUpdate;
    }
}