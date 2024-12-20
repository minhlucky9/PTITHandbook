using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class CharacterStats : MonoBehaviour
    {
        public int characterLevel;

        public int healthLevel = 10;
        public int maxHealth;
        public int currentHealth;

        public int staminaLevel = 10;
        public float maxStamina;
        public float currentStamina;

        public bool isDead;
    }

}
