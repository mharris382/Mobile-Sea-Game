using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eel : MonoBehaviour
{
    public float maxMoveSpeed = 2;
    public float smoothing = 0.1f;
    public float rotateSmoothing = 0.1f;
    public Transform[] waypoints;


    private int _index = 0;
    private Transform _currentTarget;

    private Vector3 _smoothDampVelocity; //variable used for unity's smooth damp method
    private Vector3 _dampRotation;

    private void Update()
    {
        //should only be null when stopped chasing a target and waypoint hasn't been set yet
        if (_currentTarget == null)
            _currentTarget = GetCurrentWaypoint();

        var targetPos = _currentTarget.position;
        var curPos = transform.position;

        RotateTowardsTarget();

        transform.position = Vector3.SmoothDamp(curPos, targetPos, ref _smoothDampVelocity, smoothing, maxMoveSpeed, Time.deltaTime);
        if (WasTargetReached())
        {
            //Reset the target so that a new target will be found
            _currentTarget = null;
        }
    }

    private void RotateTowardsTarget()
    {
        //var cRot = transform.rotation;
        //var tRot = Quaternion.LookRotation(_currentTarget.position, Vector3.forward);
        //transform.rotation = Quaternion.Slerp(cRot, tRot, Time.deltaTime * rotateSpeed);
        var curr = transform.forward;
        var target = (_currentTarget.position - transform.position).normalized;
        transform.forward = Vector3.SmoothDamp(curr, target, ref _dampRotation, rotateSmoothing);
    }

    private bool WasTargetReached()
    {
        var dist = (_currentTarget.position - transform.position).sqrMagnitude;
        return (dist < (0.125f * 0.125f));

    }

    public void NextWp()
    {
        _index += 1;
        _index %= waypoints.Length;
    }

    private Transform GetCurrentWaypoint()
    {
        var wp = waypoints[_index];
        _currentTarget = wp;
        if (WasTargetReached())
        {
            NextWp();
            wp = waypoints[_index];
        }
        Debug.Assert(wp != null, "Waypoint is null", this);
        return wp;
    }
}
