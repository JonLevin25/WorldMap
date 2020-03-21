using System;
using UnityEngine;
using WorldMap.Inputs.Helpers;

namespace WorldMap.Inputs
{
    public class MapInputUnityAxes : MapInputBase
    {
        [Tooltip("Axis to select nodes with, within the map")]
        [SerializeField] private Axis2DInputMethod _navigationAxes = DefaultNavigationAxisValues();
        [Space]
        [SerializeField] private ButtonInputMethod _submitButton = DefaultSubmitValues();
        [Space]
        [SerializeField] private ButtonInputMethod _cancelButton = DefaultCancelValues();

        [Header("Camera controls (optional)")]
        [SerializeField] private MouseCameraInput _mouseCameraInput;
        [SerializeField] private Axis2DInputMethod _cameraAxes = DefaultCameraAxisValues();

        private Vector2 _prevNavInput; // Since nav is axes, previous is used for diff, to simulate `GetButtonDown`
        private bool _usingMouseInput;

        public override event Action<MapInputPayload> OnInputUpdate;

        private void Start()
        {
            _usingMouseInput = _mouseCameraInput != null;
        }

        private void OnValidate()
        {
            _usingMouseInput = _mouseCameraInput != null;
        }

        private void Update()
        {
            var rawNavInput = _navigationAxes.GetInput();
            var navInput = ProcessNavInput(rawNavInput);

            var cameraInput = _cameraAxes.GetInput();

            var submitInput = _submitButton.GetButtonDown();
            var cancelInput = _cancelButton.GetButtonDown();

            if (_usingMouseInput)
            {
                cameraInput += _mouseCameraInput.GetInput();
            }
            
            _prevNavInput = rawNavInput;
            
            // Send payload
            var payload = new MapInputPayload(navInput, cameraInput, submitInput, cancelInput);
            OnInputUpdate?.Invoke(payload);
        }

        private Vector2 ProcessNavInput(Vector2 navInput)
        {
            bool IsSameDirection(float x, float y) => x * y > 0f;
            
            var input = _navigationAxes.GetInput();
            var finalInput = input;
            
            // Simulate ButtonDown - ignore axis if same direction
            if (IsSameDirection(input.x, _prevNavInput.x)) finalInput.x = 0f;
            if (IsSameDirection(input.y, _prevNavInput.y)) finalInput.y = 0f;
            return finalInput;
        }

        private static Axis2DInputMethod DefaultNavigationAxisValues()
        {
            var horAxis = new AxisInputMethod(UnityInputMethod.KeyCode, "Horizontal", KeyCode.RightArrow, KeyCode.LeftArrow);
            var verAxis = new AxisInputMethod(UnityInputMethod.KeyCode, "Vertical", KeyCode.UpArrow, KeyCode.DownArrow); 
            
            return new Axis2DInputMethod(horAxis, verAxis);
        }
        
        private static Axis2DInputMethod DefaultCameraAxisValues()
        {
            var horAxis = new AxisInputMethod(UnityInputMethod.KeyCode, "", KeyCode.D, KeyCode.A);
            var verAxis = new AxisInputMethod(UnityInputMethod.KeyCode, "", KeyCode.W, KeyCode.S);
            
            return new Axis2DInputMethod(horAxis, verAxis);
        }

        private static ButtonInputMethod DefaultSubmitValues()
        {
            return new ButtonInputMethod(UnityInputMethod.UnityAxis, "Submit", KeyCode.Return);
        }

        private static ButtonInputMethod DefaultCancelValues()
        {
            return new ButtonInputMethod(UnityInputMethod.UnityAxis, "Cancel", KeyCode.Return);
        }
    }
}