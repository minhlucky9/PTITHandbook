﻿using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    //Scene References
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset1 Preset;
    //Variables
    [SerializeField, Range(0, 24)] private float TimeOfDay;

    [Header("Rain Effect Settings (seconds)")]
    [SerializeField] private ParticleSystem RainParticleSystem;
    [SerializeField] private Vector2 RainDelayRange = new Vector2(5f, 15f);
    [SerializeField] private Vector2 RainDurationRange = new Vector2(3f, 8f);

    private Coroutine rainLoop;

    //–– skybox mới
    [Header("Skybox Settings")]
    [SerializeField] private Material daySkybox;
    [SerializeField] private Material nightSkybox;

    [SerializeField, Range(0, 24)] private float eveningFadeStart = 18f;  
    [SerializeField, Min(0f)] private float eveningFadeDuration = 2f;   

    [SerializeField, Range(0, 24)] private float morningFadeStart = 5f;  
    [SerializeField, Min(0f)] private float morningFadeDuration = 2f;   

    private Material currentSkybox;

    private void Awake()
    {
        // Khởi tạo instance và gán vào RenderSettings
        if (daySkybox != null)
        {
            currentSkybox = new Material(daySkybox);
            RenderSettings.skybox = currentSkybox;
        }
    }

    private void OnEnable()
    {
        if (Application.isPlaying && RainParticleSystem != null)
        {
            rainLoop = StartCoroutine(RainLoop());
        }
    }

    private void OnDisable()
    {
        if (rainLoop != null)
        {
            StopCoroutine(rainLoop);
        }
    }

    private IEnumerator RainLoop()
    {
        while (true)
        {
            // Wait a random delay before rain
            float delay = Random.Range(RainDelayRange.x, RainDelayRange.y);
            yield return new WaitForSeconds(delay);

            // Start rain
            RainParticleSystem.Play(true);

            // Rain duration
            float duration = Random.Range(RainDurationRange.x, RainDurationRange.y);
            yield return new WaitForSeconds(duration);

            // Stop emitting new particles, let existing ones finish
            RainParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void Update()
    {
        if (Preset == null)
            return;

        if (Application.isPlaying)
        {
            //(Replace with a reference to the game time)
            TimeOfDay += Time.deltaTime;
            TimeOfDay %= 24; //Modulus to ensure always between 0-24
            UpdateLighting(TimeOfDay / 24f);
        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
        }

        UpdateSkybox();
    }

    private void UpdateSkybox()
    {
        float hour = TimeOfDay;  // giả sử TimeOfDay tính theo giờ 0–24

        // 1) Morning fade: 5h → 6h
        if (hour >= morningFadeStart && hour < morningFadeStart + morningFadeDuration)
        {
            float t = (hour - morningFadeStart) / morningFadeDuration;
            currentSkybox.Lerp(nightSkybox, daySkybox, t);
        }
        // 2) Day time: sau 6h và trước 18h
        else if (hour >= morningFadeStart + morningFadeDuration && hour < eveningFadeStart)
        {
            currentSkybox.CopyPropertiesFromMaterial(daySkybox);
        }
        // 3) Evening fade: 18h → 19h
        else if (hour >= eveningFadeStart && hour < eveningFadeStart + eveningFadeDuration)
        {
            float t = (hour - eveningFadeStart) / eveningFadeDuration;
            currentSkybox.Lerp(daySkybox, nightSkybox, t);
        }
        // 4) Night time: sau 19h hoặc trước 5h
        else
        {
            currentSkybox.CopyPropertiesFromMaterial(nightSkybox);
        }

        // Cập nhật GI để phản xạ skybox mới
        DynamicGI.UpdateEnvironment();
    }




    private void UpdateLighting(float timePercent)
    {
        //Set ambient and fog
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        //If the directional light is set then rotate and set it's color, I actually rarely use the rotation because it casts tall shadows unless you clamp the value
        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);

            float xRotation = (timePercent * 360f) - 90f;

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));

            float baseIntensity = 1f;

            if (xRotation <= 160f)
            {
                DirectionalLight.intensity = baseIntensity;
            }
            else if (xRotation > 160f && xRotation < 180f)
            {
                // Giảm dần từ 1 đến 0
                float t = (xRotation - 160f) / 20f; // t ∈ (0,1)
                DirectionalLight.intensity = Mathf.Lerp(baseIntensity, 0f, t);
            }
            else // xRotation >= 180f
            {
                DirectionalLight.intensity = 0f;
            }
        }

    }



    //Try to find a directional light to use if we haven't set one
    private void OnValidate()
    {
        if (DirectionalLight != null)
            return;

        //Search for lighting tab sun
        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        //Search scene for light that fits criteria (directional)
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }

}
