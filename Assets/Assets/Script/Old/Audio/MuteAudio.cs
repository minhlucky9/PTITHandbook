using UnityEngine;

public class ToggleAudioSources : MonoBehaviour
{
    private bool isMuted = false;
    public AudioSource[] audioSources; // G?n các AudioSource qua Inspector

    public void ToggleMute()
    {
        // ??i tr?ng thái gi?a mute và unmute
        isMuted = !isMuted;

        // Duy?t qua t?t c? các AudioSource và thay ??i tr?ng thái mute
        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource.isPlaying || isMuted)
            {
                // N?u ?ang phát ho?c c?n mute, thay ??i tr?ng thái mute
                audioSource.mute = isMuted;
            }
            // N?u không phát nh?c, gi? nguyên tr?ng thái mute
        }
    }
}
