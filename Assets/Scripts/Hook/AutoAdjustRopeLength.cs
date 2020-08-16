using System;
using Diver;
using UnityEngine;
using Zenject;

public class AutoAdjustRopeLength : MonoBehaviour
{
    private RopeTest _rope;


    public float speed = 5;
    public float minRopeDistance = 1;
    public float maxRopeDistance = 50;

    private float _currentDistance;
    
    private DiverInput _input;




    [Inject]
    void Inject(DiverInput input)
    {
        this._input = input;
    }

    private void Awake()
    {
        _rope = GetComponent<RopeTest>();
        
    }

    private void Update()
    {
        var moveInput = _input.HookMoveInput;
        
        if(Mathf.Abs(moveInput) < 0.1f)
            return;
        
        // if((moveInput < 0 && _currentDistance >= maxRopeDistance) || 
        //    (moveInput > 0) && _currentDistance <= minRopeDistance)
        //     return;

        var moveAmount = Time.deltaTime * speed * moveInput;
        _currentDistance += -moveAmount;
        _rope.Distance = _currentDistance;
    }
}