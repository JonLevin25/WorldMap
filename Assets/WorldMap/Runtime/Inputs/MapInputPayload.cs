using UnityEngine;

namespace WorldMap.Inputs
{
    public struct MapInputPayload
    {
        public readonly Vector2 SelectionAxes;
        public readonly Vector2 CameraAxes;
        public readonly bool SubmitButton;
        public readonly bool CancelButton;

        public MapInputPayload(Vector2 selectionAxes, Vector2 cameraAxes, bool submitButton, bool cancelButton)
        {
            SelectionAxes = selectionAxes;
            CameraAxes = cameraAxes;
            SubmitButton = submitButton;
            CancelButton = cancelButton;
        }
    }
}