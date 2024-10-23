using UnityEngine;

public class EndTrigger : MonoBehaviour
{

    public GameM Game;

    void OnTriggerEnter()
    {
        Game.CompleteLevel();
    }

}
