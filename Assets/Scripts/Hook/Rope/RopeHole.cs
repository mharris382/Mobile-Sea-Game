
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hook
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class RopeHole : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private BoxCollider2D _box;
        
        

        private void Awake()
        {
            this._rb = GetComponent<Rigidbody2D>();
            this._box = GetComponent<BoxCollider2D>();
            
            _rb.isKinematic = true;
            _box.isTrigger = true;
            
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.attachedRigidbody.CompareTag("Hook"))
            {
                //hook is inside the hole   
                
            }
            else
            {
                
            }
        }
    }
}