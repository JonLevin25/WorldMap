using System;
using System.Linq;
using UnityEngine;

namespace GalaxyMap.Inputs.Helpers
{
    [Serializable]
    public class AxisInputMethod
    {
        [SerializeField] private UnityInputMethod _inputMethod;
        [SerializeField] private string _axisName;
        
        [SerializeField] private KeyCode _positiveKey;
        [SerializeField] private KeyCode _negativeKey;

        // ctor For setting inspector defaults if nested in another inspector/property
        public AxisInputMethod(UnityInputMethod inputMethod, string axisName, KeyCode posKey, KeyCode negKey)
        {
            _inputMethod = inputMethod;
            _axisName = axisName;
            _positiveKey = posKey;
            _negativeKey = negKey;
        }

        public float GetInput()
        {
            switch (_inputMethod)
            {
                case UnityInputMethod.UnityAxis:
                    return GetAxesInput();
                case UnityInputMethod.KeyCode:
                    return GetKeysInput();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private float GetKeysInput()
        {
            var pos = GetKeyInput(_positiveKey);
            var neg = GetKeyInput(_negativeKey);

            return pos - neg;
        }

        private float GetAxesInput()
        {
            if (!IsAxisSet(_axisName)) return 0f;
            return Input.GetAxis(_axisName);
        }

        private static float GetKeyInput(KeyCode key)
        {
            if (!IsKeySet(key)) return 0f;
            return Input.GetKey(key) ? 1f : 0f;
        }

        private static bool IsKeySet(KeyCode key) => key != KeyCode.None;
        private static bool IsAxisSet(string axisName) => !string.IsNullOrWhiteSpace(axisName);
    }
}