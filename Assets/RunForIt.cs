using UnityEngine;

public class RunForIt : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed;
    [SerializeField] private float _accel;
    [SerializeField] private Space _space;
    [SerializeField] private bool _constDirection;

    private Vector3 _initialDir;
    private void Start()
    {
        _initialDir = GetDirection();
    }
    
    private void Update()
    {
        var dir = _constDirection ? _initialDir : GetDirection();
        var delta = Time.deltaTime * _speed;
        transform.Translate(delta * dir, _space);

        _speed += Time.deltaTime * _accel;
    }

    private Vector3 GetDirection()
    {
        return (_target.position - transform.position).normalized;
    }
}
