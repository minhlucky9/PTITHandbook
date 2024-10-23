using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [Header("Level to load")]
    public string NewGameLevel;
    private string LevelToLoad;
    [SerializeField] private GameObject NoSaveGameDialog = null;


    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            LevelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(LevelToLoad);
        }
        else
        {
            NoSaveGameDialog.SetActive(true);
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
