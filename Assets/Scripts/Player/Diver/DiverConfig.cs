using UnityEngine;

namespace Player.Diver
{
    [CreateAssetMenu(menuName = "Data/DiverConfig")]
    public class DiverConfig : ScriptableObject
    {
        [Min(0)] public float moveSpeed = 2;
    }
    
    
    
}