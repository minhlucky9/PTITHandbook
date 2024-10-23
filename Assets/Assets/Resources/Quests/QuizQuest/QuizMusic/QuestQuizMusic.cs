
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestQuizMusic : QuestStep
{
    private int correctAnswersRequired = 5;

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        if (QuizMusic.instance != null)
        {
            QuizMusic.instance.OnAnswerSelected += CheckQuestCompletion;
            QuizMusic.instance.OnWrongAnswer += CheckQuestCancellation;
        }
    }

    private void OnDisable()
    {
        if (QuizMusic.instance != null)
        {
            QuizMusic.instance.OnAnswerSelected -= CheckQuestCompletion;
            QuizMusic.instance.OnWrongAnswer -= CheckQuestCancellation;
        }
    }

    private void CheckQuestCompletion()
    {
        if (QuizMusic.instance.GetScore() >= correctAnswersRequired)
        {
            FinishQuestStep();
            QuizMusic.instance.GameOver();
        }
        UpdateState();
    }

    private void CheckQuestCancellation()
    {
        if (QuizMusic.instance.GetWrongScore() >= 3)
        {
            CancelQuestStep();
        }
    }

    private void UpdateState()
    {
        if (QuizMusic.instance != null)
        {
            int currentScore = QuizMusic.instance.GetScore();
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
