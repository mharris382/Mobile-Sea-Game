using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour
{
    // public InputAction moveAction;
    public float moveSpeed = 10.0f;
    public Vector2 position;

    private Vector2 moveDirection = Vector2.zero;
    
    void Start()
    {
        // moveAction.Enable();
        position = transform.position;
    }

     void Update()
     {
         // var moveDirection = moveAction.ReadValue<Vector2>();
         position += moveDirection * moveSpeed * Time.deltaTime;
         transform.position = position;
     }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }
}