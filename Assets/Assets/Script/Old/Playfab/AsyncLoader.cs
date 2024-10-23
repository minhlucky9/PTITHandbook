using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AsyncLoader : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private GameObject loadingscreen;
    [SerializeField] private GameObject Menu;


    [Header("Slider")]
    [SerializeField] private Slider loadingSlider;

    public void LoadlevelBtn(string leveltoload)
    {
        Menu.SetActive(false);
        loadingscreen.SetActive(true);

        StartCoroutine(LoadLevelAsync(leveltoload));
    }

    IEnumerator LoadLevelAsync(string leveltoload)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(leveltoload);

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;
            yield return null;
        }
    }
}
