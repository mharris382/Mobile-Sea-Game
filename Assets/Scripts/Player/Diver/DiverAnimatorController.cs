using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Animator))]
public class DiverAnimatorController : MonoBehaviour
{
    private Animator _anim;
    private bool _faceLeft = true;

    private string a_IsMoving = "IsMoving";
    private string a_FaceLeft = "FaceLeft";
    private string a_VelX = "VelocityX";
    private string a_VelY = "VelocityY";
    


    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        var movement = context.ReadValue<Vector2>();
        bool isMoving = movement != Vector2.zero;
        _anim.SetBool(a_IsMoving, isMoving);
        if(!isMoving)
            return;
        if (movement.x != 0)
        {
            _faceLeft = movement.x < 0;
        }
        _anim.SetBool(a_FaceLeft, _faceLeft);
        _anim.SetFloat(a_VelX, movement.x);
        _anim.SetFloat(a_VelY, movement.y);
    }
}
