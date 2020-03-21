using UnityEngine;

namespace WorldMap.Inputs
{
    public struct MapInputPayload
    {
        public readonly Vector2 Selection;
        public readonly Vector2 Camera;
        public readonly bool SubmitButton;
        public readonly bool CancelButton;

        public MapInputPayload(Vector2 selection, Vector2 camera, bool submitButton, bool cancelButton)
        {
            Selection = selection;
            Camera = camera;
            SubmitButton = submitButton;
            CancelButton = cancelButton;
        }
    }
}