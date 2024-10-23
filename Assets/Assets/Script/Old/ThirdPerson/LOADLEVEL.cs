using UnityEngine;
using UnityEngine.SceneManagement;

public class LOADLEVEL : MonoBehaviour
{

    public void Load()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}