using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AudioFadeInGame : MonoBehaviour
{
    public AudioSource audioSource1; // Audio source ??u tiên
    public AudioSource audioSource2; // Audio source th? hai
    public Button fadeOutButton;
    public float fadeDuration = 2.0f;

    void Start()
    {
        fadeOutButton.onClick.AddListener(StartFadeOut);
    }

    void StartFadeOut()
    {
        StartCoroutine(FadeOutAudio());
    }

    IEnumerator FadeOutAudio()
    {
        float startVolume = audioSource1.volume;

        while (audioSource1.volume > 0)
        {
            audioSource1.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource1.volume = 0;
        audioSource1.Stop(); // D?ng audio source ??u tiên khi âm l??ng b?ng 0
        audioSource2.volume = 0.5f;
        audioSource2.Play(); // Kích ho?t audio source th? hai
    }
}
