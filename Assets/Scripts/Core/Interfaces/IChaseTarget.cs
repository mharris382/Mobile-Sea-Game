using UnityEngine;

namespace Core
{
    public interface IChaseTarget
    {
        Transform transform { get; }
        
        Rigidbody2D rigidbody2D { get; }
        
    }
}