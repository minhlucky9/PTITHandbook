using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MazeToTheTopQuestStep : QuestStep
{
    public int mazeCollected = 0;
    private int mazeToComplete = 1;

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        GameEventsManager.instance.miscEvents.onMazeToTheTopCollected += MazeCollected;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.miscEvents.onMazeToTheTopCollected -= MazeCollected;
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
        this.mazeCollected = System.Int32.Parse(state);
        UpdateState();
    }
}
