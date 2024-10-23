
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestQuizKTX_B5 : QuestStep
{
    private int correctAnswersRequired = 8;

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        if (QuizKTX_B5.instance != null)
        {
            QuizKTX_B5.instance.OnAnswerSelected += CheckQuestCompletion;
            QuizKTX_B5.instance.OnWrongAnswer += CheckQuestCancellation;
        }
    }

    private void OnDisable()
    {
        if (QuizKTX_B5.instance != null)
        {
            QuizKTX_B5.instance.OnAnswerSelected -= CheckQuestCompletion;
            QuizKTX_B5.instance.OnWrongAnswer -= CheckQuestCancellation;
        }
    }

    private void CheckQuestCompletion()
    {
        if (QuizKTX_B5.instance.GetScore() >= correctAnswersRequired)
        {
            FinishQuestStep();
            QuizKTX_B5.instance.GameOver();
        }
        UpdateState();
    }

    private void CheckQuestCancellation()
    {
        if (QuizKTX_B5.instance.GetWrongScore() >= 3)
        {
            CancelQuestStep();
        }
    }

    private void UpdateState()
    {
        if (QuizKTX_B5.instance != null)
        {
            int currentScore = QuizKTX_B5.instance.GetScore();
            string state = currentScore.ToString();
            string status = "Correct Answers: " + currentScore + " / " + correctAnswersRequired;
            ChangeState(state, status);
        }
    }

    protected override void SetQuestStepState(string state)
    {
        int savedScore;
        /*
        if (int.TryParse(state, out savedScore) && QuizA1.instance != null)
        {
            // Assuming we want to directly set the score in the QuizManager for some reason
            while (QuizA1.instance.GetScore() < savedScore)
            {
                QuizA1.instance.score = savedScore; // Manually incrementing score for restoration
            }
            UpdateState();
        }
        */
    }

    private void CancelQuestStep()
    {
      
        Debug.Log("Quest canceled due to too many wrong answers.");
        // Add any additional logic needed for quest cancellation
    }
}
