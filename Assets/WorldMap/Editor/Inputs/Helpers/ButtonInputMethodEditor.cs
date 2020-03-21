using System;
using UnityEditor;
using UnityEngine;
using WorldMap.Inputs.Helpers;
using static WorldMap.Editor.CustomInspectorHelpers;

namespace WorldMap.Editor.Inputs.Helpers
{
    [CustomPropertyDrawer(typeof(ButtonInputMethod))]
    public class ButtonInputMethodEditor : PropertyDrawer
    {
        /* Used for implementing "showIf" functionality.
         * Once NaughtyAttributes is updated, Should be able to use
         * [ShowIf(...), AllowNesting] and get rid of this (AllowNesting missing in 1.0.4)
         */
        
        private SerializedProperty _methodProp;
        private SerializedProperty _axisNameProp;
        private SerializedProperty _keyProp;
        
        private UnityInputMethod _inputMethod;
        private float _height;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _methodProp = property.FindPropertyRelative("_inputMethod");
            _axisNameProp = property.FindPropertyRelative("_axisName");
            _keyProp = property.FindPropertyRelative("_key");
            
            var startTop = position.yMin;
            SetToLineHeight(ref position);
            
            // EditorGUI.LabelField(position, label);
            // MoveDownLine(ref position);
            EditorGUI.PropertyField(position, _methodProp);
            // EditorGUI.PropertyField(position, _methodProp, label);
            MoveDownLine(ref position);
            
            var inputMethod = (UnityInputMethod) _methodProp.enumValueIndex;
            switch (inputMethod)
            {
                case UnityInputMethod.UnityAxis:
                    RenderProperties(ref position, _axisNameProp);
                    break;
                case UnityInputMethod.KeyCode:
                    RenderProperties(ref position, _keyProp);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            MoveUpLine(ref position); // Fix last newLine
            
            property.serializedObject.ApplyModifiedProperties();
            var bottom = position.yMax;
            _height = bottom - startTop;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => _height;
    }
}