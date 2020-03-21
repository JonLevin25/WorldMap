using UnityEditor;
using UnityEngine;

namespace GalaxyMap.Editor
{
    public static class CustomInspectorHelpers
    {
        public static void SetToLineHeight(ref Rect rect)
        {
            rect.height = EditorGUIUtility.singleLineHeight;
        }

        public static void RenderProperties(ref Rect rect, params SerializedProperty[] properties)
        {
            foreach (var prop in properties)
            {
                EditorGUI.PropertyField(rect, prop);
                MoveDownLine(ref rect);
            }
        }
        
        public static void MoveDownLine(ref Rect rect, int lines = 1)
        {
            var pos = rect.position;
            pos.y += EditorGUIUtility.singleLineHeight * lines;
            rect.position = pos;
        }

        public static void MoveUpLine(ref Rect rect) => MoveDownLine(ref rect, -1);
    }
}
