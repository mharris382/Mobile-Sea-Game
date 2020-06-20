using System;
using System.Collections;
using System.Collections.Generic;
using PolyNav;
using UnityEngine;

public class SubController : MonoBehaviour
{

    public PolyNav.PolyNav2D _map;
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

    private List<Vector2> _activePath;
    private Vector2 _primeGoal;
    private int _requests;

    public bool pathPending => _requests > 0 && !hasPath;
    
    public bool hasPath => _activePath != null && _activePath.Count > 0;
    
    public Vector2 primeGoal => _primeGoal;

    public Vector2 nextPoint => hasPath ? _activePath[0] : position;

    public Vector2 position => transform.position;

    public PolyNav2D map
    {
        get { return _map ?? (_map = FindObjectOfType<PolyNav2D>()); }
    }
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
           SetDestination(moveToTarget.position);
        }
    }


    private void FixedUpdate()
    {
        if (hasPath == false) return;
        
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


    public void SetDestination(Vector2 goal)
    {
        if(map == null)
            return;

        if ((primeGoal - goal).sqrMagnitude < Mathf.Epsilon)
        {
            return;
        }

        _primeGoal = goal;

        if (!map.PointIsValid(primeGoal))
        {
            SetDestination(map.GetCloserEdgePoint(primeGoal));
            return;
        }

        _requests++;
        map.FindPath(this.position, primeGoal, SetPath);
    }


    void SetPath(Vector2[] path)
    {
        if(_requests==0)
            return;
        if(path == null || path.Length == 0)
            return;
        
        _requests--;
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


namespace Utilities
{
}