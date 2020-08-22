using System;
using System.Collections;
using System.Collections.Generic;
using Diver;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;


[RequireComponent(typeof(Animator))]
public class DiverAnimatorController : MonoBehaviour
{
    private Animator _anim;
    private bool _faceLeft = true;


    [Range(0, 60), Tooltip("Applies smooth damp function to the movement input values before passing them to the animator, set to zero to disable damping")]
    public float smoothingAmount = 0;


    private static readonly int FaceLeft = Animator.StringToHash("FaceLeft");
    private static readonly int VelocityForwards = Animator.StringToHash("VelocityX");
    private static readonly int VelocityY = Animator.StringToHash("VelocityY");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");

    private Vector2 _target;
    private Vector2 _curr;
    private Vector2 _vel;
    private DiverInput _input;
    private bool IsUsingSmoothing => Math.Abs(smoothingAmount) > Mathf.Epsilon;


    [Inject]
    void Inject(DiverInput input)
    {
        this._input = input;
    }

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }


    private IEnumerator Start()
    {
        if (!IsUsingSmoothing) yield break;
        
        if (_input != null)
            Observable.EveryUpdate().Select(_ => _input.DiverMoveInput).DistinctUntilChanged().Subscribe(UpdateMovement);
        
        while (true)
        {
            _curr = Vector2.SmoothDamp(_curr, _target, ref _vel, smoothingAmount * Time.deltaTime);
            _anim.SetFloat(VelocityForwards, _curr.x);
            _anim.SetFloat(VelocityY, _curr.y);
            yield return null;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var movement = context.ReadValue<Vector2>();
        UpdateMovement(movement);
    }

    private void UpdateMovement(Vector2 movement)
    {
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
            _target = movement;
            return;
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