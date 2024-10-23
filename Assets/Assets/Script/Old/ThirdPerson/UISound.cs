using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSoundPlayer : MonoBehaviour, ISelectHandler
{
    public AudioClip selectSound;
    private AudioSource audioSource;

    void Start()
    {
        // Tìm AudioSource trên cùng một GameObject, nếu không có thì thêm mới
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Phát âm thanh khi button được chọn
        if (selectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(selectSound);
        }
    }
}
