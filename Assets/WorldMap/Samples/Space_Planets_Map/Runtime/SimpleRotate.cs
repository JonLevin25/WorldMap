using UnityEngine;

public class SimpleRotate : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private Vector3 _axis;
    [SerializeField] private Space _relativeTo;
    
    private void Update()
    {
        var delta = _rotationSpeed * Time.deltaTime;
        transform.Rotate(delta * _axis, _relativeTo);
    }
}