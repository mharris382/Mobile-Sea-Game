using UnityEngine;

namespace Player.Diver
{
    [CreateAssetMenu(menuName = "Data/DiverConfig")]
    public class DiverConfig : ScriptableObject
    {
        [Min(0)] public float moveSpeed = 2;


        [Tooltip("Depth at which flashlight will be always be enabled")]
        [SerializeField]private int flashlightDepth = 10;

        [SerializeField] private float maxHookHoldDistance = .75f;

        private static DiverConfig _instance;

        public static DiverConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<DiverConfig>("DiverConfig");
                }

                return _instance;
            }
        }


        public static float FlashlightDepth => Instance. flashlightDepth;

        public static float MaxHookHoldDistance => Instance.maxHookHoldDistance;
    }
    
    
    
}