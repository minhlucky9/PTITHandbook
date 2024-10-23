
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestQuiz_ThuVien : QuestStep
{
    private int correctAnswersRequired = 8;

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        if (Quiz_ThuVien.instance != null)
        {
            Quiz_ThuVien.instance.OnAnswerSelected += CheckQuestCompletion;
            Quiz_ThuVien.instance.OnWrongAnswer += CheckQuestCancellation;
        }
    }

    private void OnDisable()
    {
        if (Quiz_ThuVien.instance != null)
        {
            Quiz_ThuVien.instance.OnAnswerSelected -= CheckQuestCompletion;
            Quiz_ThuVien.instance.OnWrongAnswer -= CheckQuestCancellation;
        }
    }

    private void CheckQuestCompletion()
    {
        if (Quiz_ThuVien.instance.GetScore() >= correctAnswersRequired)
        {
            FinishQuestStep();
            Quiz_ThuVien.instance.GameOver();
        }
        UpdateState();
    }

    private void CheckQuestCancellation()
    {
        if (Quiz_ThuVien.instance.GetWrongScore() >= 3)
        {
            CancelQuestStep();
        }
    }

    private void UpdateState()
    {
        if (Quiz_ThuVien.instance != null)
        {
            int currentScore = Quiz_ThuVien.instance.GetScore();
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
