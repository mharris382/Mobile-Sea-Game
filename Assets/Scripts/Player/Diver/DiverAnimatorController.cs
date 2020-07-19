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

    [Range(0, 2), Tooltip("Applies smooth damp function to the movement input values before passing them to the animator, set to zero to disable damping")]
    public float smoothingAmount = 0;
    

    private static readonly int FaceLeft = Animator.StringToHash("FaceLeft");
    private static readonly int VelocityForwards = Animator.StringToHash("VelocityX");
    private static readonly int VelocityY = Animator.StringToHash("VelocityY");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");

    
    private Vector2 _vel;
    private bool IsUsingSmoothing => Math.Abs(smoothingAmount) > Mathf.Epsilon;
    
    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }



    public void OnMove(InputAction.CallbackContext context)
    {
        var movement = context.ReadValue<Vector2>();
        bool isMoving = movement != Vector2.zero;
        _anim.SetBool(IsMoving, isMoving);

        if (!isMoving)
        {
            HandleNoMovement();
            return;
        }
        UpdateFacingLeft(movement);
        movement.x = Mathf.Abs(movement.x);

        if (IsUsingSmoothing)
        {
            var curr = new Vector2(_anim.GetFloat(VelocityForwards), _anim.GetFloat(VelocityY));
            var target = movement.normalized;
            movement = Vector2.SmoothDamp(curr, target, ref _vel, smoothingAmount);
        }
        
        _anim.SetFloat(VelocityForwards, movement.x);
        _anim.SetFloat(VelocityY, movement.y);
    }



    private void UpdateFacingLeft(Vector2 movement)
    {
        if (Math.Abs(movement.x) > Mathf.Epsilon)
            _faceLeft = movement.x < 0;
        _anim.SetBool(FaceLeft, _faceLeft);
    }

    
    private void HandleNoMovement()
    {
        _anim.SetFloat(VelocityForwards, 1);
        _anim.SetFloat(VelocityY, 0);
        ResetDampening();
    }

    private void ResetDampening()
    {
        _vel = Vector2.zero;
    }
}