using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{

    public class HookController : MonoBehaviour
    {
      
        public OldHook oldHook;
        public Rigidbody2D diver;
        public new Camera camera;

        private void Awake()
        {
            camera = Camera.main;
        }


        public bool hookIsDropped;
        public bool withdrawingHook;

        private Vector2 hookMovePoint;

        public void OnHookRequested(InputAction.CallbackContext context)
        {
            if (hookIsDropped)
            {
                return;
            }

            hookIsDropped = true;
            var spawnPoint = diver.position;
            var surfacePoint = new Vector2(spawnPoint.x, 0);
            
            
            oldHook.gameObject.SetActive(true);
            
        }


       
       
    }


}