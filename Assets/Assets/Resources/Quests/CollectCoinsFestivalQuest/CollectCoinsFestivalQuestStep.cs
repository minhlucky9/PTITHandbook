using TMPro;
using UnityEngine;

public class CollectCoinsFestivalQuestStep : QuestStep
{
    public static CollectCoinsFestivalQuestStep Instance;
    public int coinsCollected = 0;
    public int coinsToComplete = 10;

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        if (StarFestival.instance != null)
        {
            GameEventsManager.instance.miscEvents.onStarFestivalCollected += CoinCollected;
        }
        else
        {
            Debug.LogError("Star.instance is null in CollectCoinsQuestStep OnEnable.");
        }
    }

    private void OnDisable()
    {
        if (StarFestival.instance != null)
        {
            GameEventsManager.instance.miscEvents.onStarFestivalCollected -= CoinCollected;
        }
    }

    private void CoinCollected()
    {
        if (StarFestival.instance != null && StarFestival.instance.GetStar() < coinsToComplete)
        {
            coinsCollected++;
            UpdateState();
        }

        if (StarFestival.instance != null && StarFestival.instance.GetStar() >= coinsToComplete)
        {
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
        if (StarFestival.instance != null)
        {
            string state = coinsCollected.ToString();
            string status = "Collected " + coinsCollected + " / " + coinsToComplete + " coins.";
            ChangeState(state, status);
        }
    }

    protected override void SetQuestStepState(string state)
    {
        if (StarFestival.instance != null)
        {
            this.coinsCollected = System.Int32.Parse(state);
            UpdateState();
        }
    }
}
