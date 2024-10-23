using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioFadeOut : MonoBehaviour
{
    public AudioSource audioSource;
    public Button fadeOutButton;
    public float fadeDuration = 2.0f; // Th?i gian ?? gi?m âm l??ng v? 0

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
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;

            yield return null;
        }

        audioSource.volume = 0;
    }
}