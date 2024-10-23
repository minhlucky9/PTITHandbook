using UnityEngine;
using UnityEngine.SceneManagement;

public class GameM : MonoBehaviour
{
    public AudioSource GOAL;
    bool gameHasEnded = false;

    public float restartDelay = 1f;

    public GameObject completeLevelUI;
    
    public void CompleteLevel()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in allAudioSources)
        {
            audioSource.Stop(); // D?ng t?ng âm thanh riêng l?
        }
        completeLevelUI.SetActive(true);
  
        GOAL.Play();
        
    }

    public void EndGame()
    {
        if (gameHasEnded == false)
        {
            gameHasEnded = true;
            Debug.Log("GAME OVER");
            Invoke("Restart", restartDelay);
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
