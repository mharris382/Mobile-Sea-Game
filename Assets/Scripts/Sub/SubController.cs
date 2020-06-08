using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubController : MonoBehaviour
{

    public Transform moveToTarget;

    public float maxHorizontalSpeed = 15f;
    public float maxDescentSpeed = 5f;
    [Range(0,1)]
    public float horizontalSmoothing = 0.1f;
    [Range(0, 1)]
    public float descentSmoothing = 0.5f;

    //public float changeDirectionTime = 0.25f;


    private bool _changingDirections;

    private Vector3 _targetPostion;
    private Vector3 _moveVelocity;


    private Rigidbody2D _rb;
    private SpriteRenderer _sr;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _targetPostion = transform.position;
    }

    private void Update()
    {
        if(moveToTarget != null)
        {
            _targetPostion = moveToTarget.position;
        }
    }


    private void FixedUpdate()
    {
        var targetPos = _changingDirections ? (transform.position + _moveVelocity) : this._targetPostion;

        var targetX = _targetPostion.x;
        var curX = transform.position.x;
        var xDir = curX - targetX;
        CheckForDirectionChange(xDir);
        var newX = Mathf.SmoothDamp(curX, targetX, ref _moveVelocity.x, horizontalSmoothing, maxHorizontalSpeed);

        var curY = transform.position.y;
        var targetY = _targetPostion.y;
        var newY = Mathf.SmoothDamp(curY, targetY, ref _moveVelocity.y, descentSmoothing, maxDescentSpeed);

        _rb.position = new Vector2(newX, newY);


    }

    private void CheckForDirectionChange(float xDir)
    {
        bool lastDir = _sr.flipX;
        _sr.flipX = xDir > 0;
        //if (_sr.flipX != lastDir)
        //{
        //    FlipDirections();
        //}
    }

    //private void FlipDirections()
    //{
    //    _changingDirections = true;
    //    Invoke("CompleteFlip", changeDirectionTime);
    //}

    //private void CompleteFlip()
    //{
    //    _changingDirections = false;
    //}
}
