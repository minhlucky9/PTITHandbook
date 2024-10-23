
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestQuizA1 : QuestStep
{
    private int correctAnswersRequired = 8;

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        if (QuizA1.instance != null)
        {
            QuizA1.instance.OnAnswerSelected += CheckQuestCompletion;
            QuizA1.instance.OnWrongAnswer += CheckQuestCancellation;
        }
    }

    private void OnDisable()
    {
        if (QuizA1.instance != null)
        {
            QuizA1.instance.OnAnswerSelected -= CheckQuestCompletion;
            QuizA1.instance.OnWrongAnswer -= CheckQuestCancellation;
        }
    }

    private void CheckQuestCompletion()
    {
        if (QuizA1.instance.GetScore() >= correctAnswersRequired)
        {
            FinishQuestStep();
            QuizA1.instance.GameOver();
        }
        UpdateState();
    }

    private void CheckQuestCancellation()
    {
        if (QuizA1.instance.GetWrongScore() >= 3)
        {
            CancelQuestStep();
        }
    }

    private void UpdateState()
    {
        if (QuizA1.instance != null)
        {
            int currentScore = QuizA1.instance.GetScore();
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
