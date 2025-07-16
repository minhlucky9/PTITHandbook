using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSaveManager : MonoBehaviour
{
    // Tham chi?u ??n script PlayerDataManager
    private PlayerDataManager dataManager;

    void Awake()
    {
        // Gi? GameObject n�y su?t ??i ?ng d?ng
        DontDestroyOnLoad(gameObject);

        dataManager = FindObjectOfType<PlayerDataManager>();
        // ??ng k� event
        Application.quitting += SaveOnQuit;
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveOnQuit();
    }

    void SaveOnQuit()
    {
        if (dataManager != null)
            dataManager.fomo();  // g?i h�m post d? li?u l�n server :contentReference[oaicite:0]{index=0}
    }
}
