using System;
using UnityEngine;

namespace GalaxyMap.Inputs.Helpers
{
    [Serializable]
    public class ButtonInputMethod
    {
        [SerializeField] private UnityInputMethod _inputMethod;
        [SerializeField] private string _axisName;
        [SerializeField] private KeyCode _key;

        // ctor For setting inspector defaults if nested in another inspector/property
        public ButtonInputMethod(UnityInputMethod inputMethod, string axisName, KeyCode key)
        {
            _axisName = axisName;
            _key = key;
            _inputMethod = inputMethod;
        }

        public bool GetButtonDown()
        {
            switch (_inputMethod)
            {
                case UnityInputMethod.UnityAxis: return Input.GetButtonDown(_axisName);
                case UnityInputMethod.KeyCode:   return Input.GetKeyDown(_key);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public bool GetButton()
        {
            switch (_inputMethod)
            {
                case UnityInputMethod.UnityAxis: return Input.GetButton(_axisName);
                case UnityInputMethod.KeyCode:   return Input.GetKey(_key);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        

        public bool GetButtonUp()
        {
            switch (_inputMethod)
            {
                case UnityInputMethod.UnityAxis: return Input.GetButtonUp(_axisName);
                case UnityInputMethod.KeyCode:   return Input.GetKeyUp(_key);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}