using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


public class DemoCamShotRunner : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCam;
    [SerializeField] private float _waypointSpeed = 1f;
    [SerializeField] private List<float> _speedOverrides;
        
    private CinemachineTrackedDolly _dollyComponent;

    private void Start()
    {
        _dollyComponent = _virtualCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        _dollyComponent.m_PositionUnits = CinemachinePathBase.PositionUnits.PathUnits;
        _dollyComponent.m_PathPosition = 0f;
    }

    // Update is called once per frame
    private void Update()
    {
        var currSegment = Mathf.FloorToInt(_dollyComponent.m_PathPosition);
        
        var speed = GetSpeed(currSegment);
        var delta = Time.deltaTime * speed;
        _dollyComponent.m_PathPosition += delta;
    }

    private float GetSpeed(int segment)
    {
        var hasOverride = _speedOverrides?.Count > segment && _speedOverrides[segment] != 0f;
        return hasOverride ? _speedOverrides[segment] : _waypointSpeed;
    }
}
