using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class BreakOnCollision : MonoBehaviour
{
    [Tooltip("The mass of incoming body required to break this object")]
    public float massThreshold = 3;

   
    
    [SerializeField] private BreakMode breakMode = BreakMode.Dynamic;
    
    [ShowIf("@breakMode==BreakMode.BreakParts")]
    [SerializeField] private Rigidbody2D[] breakParts;
    public UnityEvent onBreak;
    
    
    private Rigidbody2D _rb;
    
    
    
    
    [Flags]
    private enum BreakMode
    {
        Deactivate,
        Dynamic,
        BreakParts
    }

    private void Awake()
    {
        this._rb = GetComponent<Rigidbody2D>();
        _rb.isKinematic = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var incomingBody = other.collider.attachedRigidbody;
        
        if(incomingBody.mass < massThreshold)
            return;

        Break();
    }
    
    
    

    private void Break()
    {
        
        switch (breakMode)
        {
            case BreakMode.Deactivate:
                gameObject.SetActive(false);
                break;
            case BreakMode.Dynamic:
                _rb.isKinematic = false;
                break;
            case BreakMode.BreakParts:
                foreach (var part in breakParts)
                {
                    part.gameObject.SetActive(true);
                    part.transform.SetParent(null);
                    part.isKinematic = false;
                }
                gameObject.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

//
// public class BreakOnCollision2 : MonoBehaviour
// {
//     [Flags]
//     private enum BreakMode
//     {
//         Deactivate,
//         Dynamic,
//         BreakParts
//     }
//     
//     
//     [Tooltip("The mass of incoming body required to be counted towards the carried mass of this object")]
//     public float massThreshold = 1;
//
//     [Tooltip("Total mass of all colliding bodies required to break object")]
//     public float breakLimit = 5;
//     
//     public UnityEvent onBreak;
//     
//     [SerializeField] private BreakMode breakMode = BreakMode.Dynamic;
//     
//     [ShowIf("@_breakMode==BreakMode.BreakParts")]
//     [SerializeField] private Rigidbody2D[] breakParts;
//
//     private Rigidbody2D _rb;
//     private bool _isBroken = false;
//     
//     
//     
//    private Dictionary<Rigidbody2D, float> _carryingWeight = new Dictionary<Rigidbody2D, float>();
//    private float TotalMass => _carryingWeight.Sum(t => t.Value);
//     private void Awake()
//     {
//         this._rb = GetComponent<Rigidbody2D>();
//         _rb.isKinematic = true;
//     }
//
//     private void OnCollisionEnter2D(Collision2D other)
//     {
//         if (_isBroken == true) return;
//         
//         var incomingBody = other.collider.attachedRigidbody;
//
//         if (incomingBody.mass < massThreshold)
//         {
//             if (_carryingWeight.ContainsKey(other.rigidbody)) return;
//             
//             _carryingWeight.Add(other.rigidbody, other.rigidbody.mass);
//         }
//
//         if (TotalMass < breakLimit) return;
//         
//
//         Break();
//     }
//
//
//     
//     
//
//     private void Break()
//     {
//         _isBroken = true;
//         switch (breakMode)
//         {
//             case BreakMode.Deactivate:
//                 gameObject.SetActive(false);
//                 break;
//             case BreakMode.Dynamic:
//                 _rb.isKinematic = false;
//                 break;
//             case BreakMode.BreakParts:
//                 foreach (var part in breakParts)
//                 {
//                     part.gameObject.SetActive(true);
//                     part.transform.SetParent(null);
//                     part.isKinematic = false;
//                 }
//                 gameObject.SetActive(false);
//                 break;
//             default:
//                 throw new ArgumentOutOfRangeException();
//         }
//     }
// }
