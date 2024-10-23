using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizGameManager : MonoBehaviour
{
    public AudioSource CollisionSound;
    public AudioSource MoveSound2;
    void OnCollisionEnter(Collision collision)
    {
        // Kiểm tra xem va chạm có xảy ra với vật thể mà bạn muốn phát âm thanh hay không
        if (collision.gameObject.CompareTag("Wall")) // Thay "YourTagHere" bằng tag của vật thể bạn muốn kiểm tra va chạm
        {
            // Phát âm thanh mới khi va chạm xảy ra
            if (!CollisionSound.isPlaying) // Kiểm tra xem âm thanh đã được phát chưa để tránh việc phát lại trong khi vẫn còn đang phát
            {
                CollisionSound.Play();
                MoveSound2.enabled = false;
            }
        }
    }

    private void Update()
    {
        if( Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) 
        { 
            MoveSound2.enabled = true;
        }
        else
        {
            MoveSound2.enabled = false;
        }
    }
}
