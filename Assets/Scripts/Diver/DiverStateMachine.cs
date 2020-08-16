using System;
using Holdables;
using UnityEngine;
using Zenject;
using UniRx;
namespace Diver
{
    public class DiverStateMachine : MonoBehaviour
    {
        private Holder _diverHolder;
        
        [Inject]
        void Inject(Holder diverHolder, Holdables.Diver.HeavyHoldableListener heavyHoldableListener)
        {
            _diverHolder = diverHolder;
            
            heavyHoldableListener.DiverIsCarryingHeavyHoldable.Subscribe(t =>
            {
                Debug.Log(t ? "Diver is now carrying heavy holdable" : "Diver is not carrying heavy holdable");
            });
        }    
    }
}