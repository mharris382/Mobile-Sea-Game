using System;
using Core;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// tracks the depth of an object (in feet below sea level)
    /// </summary>
    public class DepthTracker : MonoBehaviour
    {

        private ObservedValue<int> _depth;

        public event Action<Depth> OnDepthChanged;
        
        
        public Depth CurrentDepth => _depth.Value;

        
        private void Awake()
        {
            _depth = new ObservedValue<int>(0);
            _depth.OnValueChanged += i => { OnDepthChanged?.Invoke(i); };
        }

        private void Start() => _depth.Value = GetCurrentDepth();

        private int GetCurrentDepth() => Mathf.CeilToInt(transform.position.y);

        private void LateUpdate() => _depth.Value = GetCurrentDepth();
    }

    
    /// <summary>
    /// struct for tracking depth
    /// </summary>
    public struct Depth
    {
        public bool Equals(Depth other)
        {
            return feetBelowSeaLvl == other.feetBelowSeaLvl;
        }

        public override bool Equals(object obj)
        {
            return obj is Depth other && Equals(other);
        }

        public override int GetHashCode()
        {
            return feetBelowSeaLvl;
        }

        private int _ftBelowSeaLvl;
        public int feetBelowSeaLvl
        {
            get => _ftBelowSeaLvl;
            set => _ftBelowSeaLvl = value;
        }


        public static implicit operator Depth(int i)
        {
            return new Depth()
            {
                feetBelowSeaLvl = -i
            };
        }

        public static implicit operator int(Depth d)
        {
            return d.feetBelowSeaLvl;
        }
    }
}