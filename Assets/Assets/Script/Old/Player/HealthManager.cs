using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalResponseData_Login;
public class HealthManager : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private float healthDeductionRate =0.01f; // Amount to reduce each hour
    [SerializeField] private GoldManager goldManager;
    [SerializeField] private GameObject UIIngame;
    [SerializeField] private GameObject NotifHeath;

    [Header("MovementFreezee")]
    public scr_CameraController CameraToggle;
    public scr_CameraController CameraToggle2;
    public scr_PlayerController MovementToggle;
    public scr_PlayerController MovementToggle2;
    public Character3D_Manager_Ingame character;
    public MouseManager MouseManager;
    public ButtonActivator ButtonActivator;

    public GameObject death;
    private void Start()
    {
        if( GlobalResponseData.FirstTimeQuest == 1)
        {
            healthBar.slider.value = GlobalResponseData.HealthSlider;
        }
        StartCoroutine(DeductHealthOverTime());
    }

    private IEnumerator DeductHealthOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1); // Wait for one hour (3600 seconds)
            healthBar.SetHealth(healthBar.slider.value - healthDeductionRate);
        }
    }

    public void Death()
    {
        if(healthBar.slider.value <= 0)
        {
            MouseManager.ShowCursor();
            ButtonActivator.IsUIShow = true;
            death.SetActive(true);
            if (character.index == 0)
            {
                MovementToggle.isCheck = false;
                CameraToggle.isCheck = false;
            }
            else
            {
                MovementToggle2.isCheck = false;
                CameraToggle2.isCheck = false;
            }
            UIIngame.SetActive(false);
        }
    }
    public void NotifForHeath()
    {
        if(healthBar.slider.value == 30.00042f) 
        {
            MouseManager.ShowCursor();
            ButtonActivator.IsUIShow = true;
            NotifHeath.SetActive(true);
            if (character.index == 0)
            {
                MovementToggle.isCheck = false;
                CameraToggle.isCheck = false;
            }
            else
            {
                MovementToggle2.isCheck = false;
                CameraToggle2.isCheck = false;
            }
            UIIngame.SetActive(false);
        }
    }

    public void resurrect()
    {
        healthBar.SetHealth(100);
        goldManager.SpendGold(goldManager.currentGold - (goldManager.currentGold /3) );
    }

    private void Update()
    {
        Death();
        NotifForHeath();
    }
}
