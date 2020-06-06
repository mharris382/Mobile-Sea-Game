﻿using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

namespace Diver
{
    public class DiverSmoothMovement : MonoBehaviour
    {
        [Min(0)] public float moveSpeed = 2;

        private Vector2 _moveDirection = Vector2.zero;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            _rb.velocity = _moveDirection * moveSpeed;
            Vector3 currPosition = transform.position;
            ClampPositionToLevel(ref currPosition);
            transform.position = currPosition;
            // currPosition += _moveDirection * moveSpeed * Time.deltaTime;
            // transform.position = currPosition;
        }

        private void ClampPositionToLevel(ref Vector3 position)
        {
            var rect = GameManager.Instance.CurrentLevel.GetLevelRect();

            position.x = Mathf.Clamp(position.x, rect.xMin, rect.xMax);
            position.y = Mathf.Clamp(position.y, rect.yMin, rect.yMax);
        }

        // Start is called before the first frame update
        public void OnMove(InputAction.CallbackContext context)
        {
            _moveDirection = context.ReadValue<Vector2>().normalized;
        }

        public void OnMove_OnScreen(Vector2 movement)
        {
            _moveDirection = movement;
        }
    }
}

namespace UnityEngine.InputSystem.OnScreen
{
}