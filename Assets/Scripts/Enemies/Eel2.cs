using UnityEngine;

namespace enemies
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Eel2 : MonoBehaviour
    {
        
        [Header("ChaseSettings")]
        [SerializeField] private float chaseMoveSpeed = 9;
        [SerializeField] private float chaseMultiplier = 4;
        
        public Transform[] waypoints;
        
        
        private Transform _currentTarget;
        public Transform CurrentTarget
        {
            get => _currentTarget;
            private set => _currentTarget = value;
        }
    }
}