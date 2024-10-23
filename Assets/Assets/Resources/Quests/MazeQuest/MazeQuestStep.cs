using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MazeQuestStep : QuestStep
{
    public int mazeCollected = 0;
    private int mazeToComplete = 2;

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        GameEventsManager.instance.miscEvents.onMazeCollected += MazeCollected;
        GameEventsManager.instance.miscEvents.onMazeRetry += MazeRetry;

    }

    private void OnDisable()
    {
        GameEventsManager.instance.miscEvents.onMazeCollected -= MazeCollected;
        GameEventsManager.instance.miscEvents.onMazeRetry -= MazeRetry;
    }

    private void MazeCollected()
    {
        if (mazeCollected < mazeToComplete)
        {
            mazeCollected++;
            UpdateState();
        }

        if (mazeCollected >= mazeToComplete)
        {
           // FinishQuestStep();
        }
    }

    private void MazeRetry()
    {
        mazeCollected = 0;
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.CompareTag("Player") && mazeCollected >= mazeToComplete)
        {
            //  string status = "The " + pillarNumberString + " pillar has been visited.";
            //  ChangeState("", status);
            FinishQuestStep();
        }
    }
    private void UpdateState()
    {
        string state = mazeCollected.ToString();
        string status = "Collected " + mazeCollected + " / " + mazeToComplete + " coins.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {
       // this.mazeCollected = System.Int32.Parse(state);
       // UpdateState();
    }
}
