using Core;
using PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStatsController
{
    public class PlayerStats : CharacterStats
    {
        PlayerManager playerManager;
        public HealthBar healthBar;
        public FocusPointBar focusPointBar;
        public StaminaBar staminaBar;
        PlayerAnimatorHandle animatorHandle;

        public float staminaRegenerationAmount = 30;
        public float regenerationStaminaTimer = 0;

        public int soulCount;
        private void Awake()
        {
            focusPointBar = FindObjectOfType<FocusPointBar>();
            playerManager = GetComponent<PlayerManager>();
            animatorHandle = GetComponentInChildren<PlayerAnimatorHandle>();
        }

        private void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            //healthBar.SetMaxHealth(maxHealth);
            //healthBar.SetCurrentHealth(currentHealth);

            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentStamina = maxStamina;
            //staminaBar.SetMaxStamina(maxStamina);
            //staminaBar.SetCurrentStamina(currentStamina);

            maxFocusPoint = SetMaxFocusPointFromFocusLevel();
            currentFocusPoint = maxFocusPoint;
            //focusPointBar.SetMaxFocusPoint(maxFocusPoint);
            //focusPointBar.SetCurrentFocusPoint(currentFocusPoint);
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        private float SetMaxStaminaFromStaminaLevel()
        {
            maxStamina = staminaLevel * 10;
            return maxStamina;
        }

        private float SetMaxFocusPointFromFocusLevel()
        {
            maxFocusPoint = focusPointLevel * 10;
            return maxFocusPoint;
        }

        public void TakeDamageNoAnimation(int damage)
        {
            if (isDead)
                return;

            currentHealth -= damage;
            //healthBar.SetCurrentHealth(currentHealth);
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDead = true;
            }  
        }

        public void TakeDamage(int damage)
        {
            if (isDead)
                return;

            currentHealth -= damage;
            //healthBar.SetCurrentHealth(currentHealth);

            if(currentHealth <= 0)
            {
                HandleDeath();
            } else
            {
                animatorHandle.PlayTargetAnimation("Damage_01", true);
            }
        }

        private void HandleDeath()
        {
            currentHealth = 0;
            animatorHandle.PlayTargetAnimation("Death_01", true);
            isDead = true;
        }

        public void ReduceStamina(int amount)
        {
            currentStamina -= amount;
            //staminaBar.SetCurrentStamina((int)currentStamina);
        }

        public void RegenerateStamina()
        {
 
            if (playerManager.isInteracting)
            {
                regenerationStaminaTimer = 0;
            }
            else
            {
                regenerationStaminaTimer += Time.deltaTime;

                if (currentStamina < maxStamina && regenerationStaminaTimer > 1f)
                {
                    currentStamina += staminaRegenerationAmount * Time.deltaTime;
                    //staminaBar.SetCurrentStamina((int)currentStamina);
                }
            }

        }

        public void HealPlayer(int healAmount)
        {
            currentHealth = currentHealth + healAmount;

            if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

            //healthBar.SetCurrentHealth(currentHealth);
        }

        public void ReduceFocusPoint(int amount)
        {
            currentFocusPoint -= amount;

            if(currentFocusPoint < 0)
            {
                currentFocusPoint = 0;
            }

            //focusPointBar.SetCurrentFocusPoint(currentFocusPoint);
        }

        public void AddSoul(int souls)
        {
            soulCount += souls;
        }
    }
}

