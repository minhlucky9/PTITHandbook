using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerStatsController
{
    public class HealthBar : MonoBehaviour
    {
        public Slider slider;
        public Gradient gradient;
        public Image fill;

        private void Awake()
        {
            slider = GetComponent<Slider>();
        }

        public void SetMaxHealth(float maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;

            fill.color = gradient.Evaluate(1f);

        }

        public void SetCurrentHealth(float currentHealth)
        {
            slider.value = currentHealth;

            fill.color = gradient.Evaluate(slider.normalizedValue);
        }


    }
}

