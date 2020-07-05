using System;
using UnityEngine;

namespace Player
{
    public class HookHolder : MonoBehaviour, IHook
    {
        private Holder _holder;

        private void Awake()
        {
            _holder = GetComponent<Holder>();
        }
    }




    public interface IHook
    {
        
    }



    
}