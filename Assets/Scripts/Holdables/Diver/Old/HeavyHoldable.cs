using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Holdables.Diver
{
    [TypeInfoBox("Holdable that is too heavy for diver to swim with, but can still be carried horizontally")]
    public class HeavyHoldable : MonoBehaviour
    {
        internal static HashSet<IHoldable> heavyHoldables = new HashSet<IHoldable>(); 
        
        private IHoldable _holdable;
        private bool _isHeld;
        
        private void Awake()
        {
            this._holdable = GetComponent<IHoldable>();
            Debug.Assert(_holdable!=null, "Missing Holdable", this);
        }

        private void OnEnable()
        {
            heavyHoldables.Add(_holdable);
        }

        private void OnDisable()
        {
            heavyHoldables.Remove(_holdable);
        }
    }
}