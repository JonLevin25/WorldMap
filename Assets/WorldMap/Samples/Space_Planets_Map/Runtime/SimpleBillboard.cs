using UnityEngine;

public class SimpleBillboard : MonoBehaviour
{
    [SerializeField] private Vector3 _rotationEuler;
    
    private Transform _mainCam;
    private Transform MainCam
    {
        get
        {
            if (_mainCam == null)
            {
                _mainCam = Camera.main.transform;
            }

            return _mainCam;
        }
    }



	private void Update()
	{
		transform.LookAt(transform.position + MainCam.rotation * Vector3.forward,
			MainCam.rotation * Vector3.up);
        
        transform.Rotate(_rotationEuler, Space.Self);
	}
}