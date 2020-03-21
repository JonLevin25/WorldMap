using UnityEngine;


namespace GalaxyMap.Inputs
{
    public class MouseCameraInput : MonoBehaviour
    {
        [Range(0f, 0.5f)]
        [SerializeField] private float _screenEdgePercent;

        private void OnValidate()
        {
            if (_screenEdgePercent > 0.5f)
            {
                Debug.LogError($"Screen edge percent should be less than 0.5 (half the screen!) Received: {_screenEdgePercent}");
                _screenEdgePercent = 0.5f;
            }

            if (_screenEdgePercent < 0f)
            {
                Debug.LogError($"Screen edge percent cannot be less than 0! Received: {_screenEdgePercent}");
                _screenEdgePercent = 0f;
            }
        }

        public Vector2 GetInput()
        {
            var cursorPercent = GetCursorViewportPosition();
            return new Vector2(
                NormalizeInput(cursorPercent.x, _screenEdgePercent),
                NormalizeInput(cursorPercent.y, _screenEdgePercent)
                );
        }

        private static Vector2 GetCursorViewportPosition()
        {
            var result = Input.mousePosition;
            result.x /= Screen.width;
            result.y /= Screen.height;

            return result;
        }

        /// <summary>
        /// Given some axis' viewport position (screen percent), and a threshold (also viewport), <br />
        /// Return +1 or -1 if "reached the threshold" on either side of the screen, and 0 otherwise
        /// </summary>
        private static int NormalizeInput(float viewportPos, float threshold)
        {
#if UNITY_EDITOR
            // UX fix for editor:
            // If not over game window- don't keep scrolling.
            // (In game, if windowed stopping around the edge might be annoying)
            if (viewportPos > 1f || viewportPos < 0f) return 0;
#endif
                
            if (viewportPos <= threshold) return -1;
            if (viewportPos >= (1 - threshold)) return +1;
            return 0;
        }
    }
}