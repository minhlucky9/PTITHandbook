
using System;
using UnityEngine;

public static class Models 
{
    #region Player
    [Serializable]
    public class CameraSettingModel
    {
        [Header("Camera Settings")]
        public float SensitivityX;
        public bool InvertedX;

        public float SensitivityY;
        public bool InvertedY;

        public float YClampMin = 40f;
        public float YClampMax = 40f;

        [Header("Charater")]
        public float CharaterRotationSmoothDamp = 1f;
    }


    [Serializable]
    public class PlayerSettingModel
    {
        public float ForwardSpeed = 1;
        public float CharaterRotationSmoothDamp = 0.6f;

        [Header("Movement Speed")]
        public float WalkingSpeed;
        public float RunningSpeed;

        public float WalkingBackwardSpeed;
        public float RunningBackwardSpeed;

        public float WalkingStrafingSpeed;
        public float RunningStrafingSpeed;

        public float SprintingSpeed;
    }
    [Serializable]
    public class PlayerStatsModels
    {
        public float Stamina;
        public float MaxStamina;
        public float StaminaDrain;
        public float StaminaRestore;
        public float StaminaDelay;
        public float StaminaCurrentDelay;
    }
    #endregion
}
