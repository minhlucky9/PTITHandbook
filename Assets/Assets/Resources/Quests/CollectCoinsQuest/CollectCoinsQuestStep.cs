using TMPro;
using UnityEngine;

public class CollectCoinsQuestStep : QuestStep
{
    public static CollectCoinsQuestStep Instance;
    public int coinsCollected = 0;
    public int coinsToComplete = 15;

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        if (Star.instance != null)
        {
            GameEventsManager.instance.miscEvents.onStarCollected += CoinCollected;
        }
        else
        {
            Debug.LogError("Star.instance is null in CollectCoinsQuestStep OnEnable.");
        }
    }

    private void OnDisable()
    {
        if (Star.instance != null)
        {
            GameEventsManager.instance.miscEvents.onStarCollected -= CoinCollected;
        }
    }

    private void CoinCollected()
    {
        if (Star.instance != null && Star.instance.GetStar() < coinsToComplete)
        {
            coinsCollected++;
            UpdateState();
        }

        if (Star.instance != null && Star.instance.GetStar() >= coinsToComplete)
        {
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
        if (Star.instance != null)
        {
            string state = coinsCollected.ToString();
            string status = "Collected " + coinsCollected + " / " + coinsToComplete + " coins.";
            ChangeState(state, status);
        }
    }

    protected override void SetQuestStepState(string state)
    {
        if (Star.instance != null)
        {
            this.coinsCollected = System.Int32.Parse(state);
            UpdateState();
        }
    }
}
