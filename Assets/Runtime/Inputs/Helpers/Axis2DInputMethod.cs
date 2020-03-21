using System;
using System.Linq;
using UnityEngine;

namespace WorldMap.Inputs.Helpers
{
    [Serializable]
    public class Axis2DInputMethod
    {
        [SerializeField] private AxisInputMethod _horizontal;
        [Space]
        [SerializeField] private AxisInputMethod _vertical;

        // ctor For setting inspector defaults if nested in another inspector/property
        public Axis2DInputMethod(AxisInputMethod horizontal, AxisInputMethod vertical)
        {
            _horizontal = horizontal;
            _vertical = vertical;
        }
        
        public Vector2 GetInput() => new Vector2(_horizontal.GetInput(), _vertical.GetInput());
    }
}