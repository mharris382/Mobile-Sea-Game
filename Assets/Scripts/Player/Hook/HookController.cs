using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{

    public class HookController : MonoBehaviour
    {
      
        public Hook hook;
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
            
            
            hook.gameObject.SetActive(true);
            
        }


        void CanDropToPoint(Vector2 position)
        {
            var surface = new Vector2(position.x, 0);
            
        }
        
        
        // void OnPathToDiver(Vector2[] path)
        // {
        //     if (path == null || path.Length == 0)
        //     {
        //         WithdrawHook();
        //         return;
        //     }
        // }
        public void WithdrawHook()
        {
            withdrawingHook = true;
        }

        public void IsSurfaceOnScreen()
        {
            var screenSize = new Vector2(Screen.width/2f, Screen.height/2f);
            
        }
    }


   [Serializable]
    public class HookConfig
    {
        
    }
}