using Sirenix.OdinInspector;
using UnityEngine;

namespace Player.Diver
{
    [CreateAssetMenu(menuName = "Data/DiverConfig")]
    public class DiverConfig : ScriptableObject
    {
        [Min(0)] public float moveSpeed = 2;
        
        [MinValue(("moveSpeed"))]
        public float fasterMoveSpeed = 10;
        
        [Tooltip("Depth at which flashlight will be always be enabled")]
        [SerializeField]private int flashlightDepth = 10;

        [SerializeField] private float maxHookHoldDistance = .75f;


        [Title("Heavy Movement State"),HideLabel]
        public DiverHeavyMovement.Config heavyMovementSettings;
        
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


        
        public static float GetDiverMoveSpeed(bool isFasterButtonHeld)
        {
            if (PlayerControlledSettings.GetDefaultIsFastSetting())
            {
                isFasterButtonHeld = !isFasterButtonHeld;
            }

            return isFasterButtonHeld ? Instance.fasterMoveSpeed : Instance.moveSpeed;
        }
    }

    public static class PlayerControlledSettings
    {
        private const string IS_FAST_MOVE_DEFAULT_PREF = "moveDefaultIsFast";



        public static bool GetDefaultIsFastSetting()
        {
            if (!PlayerPrefs.HasKey(IS_FAST_MOVE_DEFAULT_PREF))
            {
                PlayerPrefs.SetInt(PlayerControlledSettings.IS_FAST_MOVE_DEFAULT_PREF, 0);
            }
            return  PlayerPrefs.GetInt(PlayerControlledSettings.IS_FAST_MOVE_DEFAULT_PREF) > 0;
        }

        public static void SetDefaultIsFastSetting(bool defaultIsFastMovement)
        {
            PlayerPrefs.SetInt(IS_FAST_MOVE_DEFAULT_PREF, defaultIsFastMovement ? 1 : 0);
        }
    }
    
}