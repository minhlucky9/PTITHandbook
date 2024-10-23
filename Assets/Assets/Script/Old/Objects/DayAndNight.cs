using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DayAndNight : MonoBehaviour
{
    public float rotateSpeedZ;
    public float rotateSpeedX;

    [Header("Config")]
    [SerializeField] private float respawnTimeSeconds = 8;
    [SerializeField] private int goldGained = 1;

    private BoxCollider circleCollider;
    private MeshRenderer visual;
    private Light lightComponent;

    // ??t góc g?c là 50 ??
    private const float initialAngle = 50f;
    private const float initialShadowStrength = 0.7222334f;

    private void Awake()
    {
        circleCollider = GetComponent<BoxCollider>();
        visual = GetComponentInChildren<MeshRenderer>();
        lightComponent = GetComponent<Light>();
    }

    private void FixedUpdate()
    {
        transform.Rotate(rotateSpeedX * Time.deltaTime, 0, 0);

        // Tính góc hi?n t?i t? góc g?c 50 ?? và chu?n hóa v? 0-360
        float angle = NormalizeAngle(transform.eulerAngles.x - initialAngle);

        // Tính toán shadowStrength d?a trên góc xoay
        if (angle >= 50 && angle <= 160)
        {
            // Gi?m shadowStrength t? initialShadowStrength v? 0
            lightComponent.shadowStrength = Mathf.Lerp(initialShadowStrength, 0, (angle - 50) / 110f);
        }
        else if (angle > 160 && angle <= 230)
        {
            // Duy trì shadowStrength ? 0
            lightComponent.shadowStrength = 0;
        }
        else
        {
            // T?ng shadowStrength t? 0 v? initialShadowStrength
            lightComponent.shadowStrength = Mathf.Lerp(0, initialShadowStrength, (angle > 230 ? angle - 230 : angle + 130) / 180f);
        }
       // Debug.Log("Shadow Strength: " + lightComponent.shadowStrength + " | Rotation X: " + transform.eulerAngles.x);
    }

    private float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle < 0) angle += 360;
        return angle;
    }

    
}
