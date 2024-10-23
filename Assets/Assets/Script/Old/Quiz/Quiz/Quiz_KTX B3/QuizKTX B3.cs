using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizKTX_B3 : MonoBehaviour
{
    public static QuizKTX_B3 instance; // Singleton instance
    private List<QuestionAndAnswer_KTX_B3> originalQna; // Danh sách gốc lưu trữ các câu hỏi


    public List<QuestionAndAnswer_KTX_B3> Qna;
    public GameObject[] Options;
    public int CurrentQuestions;
    public GameObject QuizPanel;
    public GameObject GoPanel;

    public TextMeshProUGUI QuestionTxt;
    public TextMeshProUGUI ScoreTxt;
    public TextMeshProUGUI timerText;

    private float timeRemaining = 300f; // 5 phút = 300 giây
    public bool isTimerRunning = false;

    int TotalQuestion = 0;
    public int score;
    private int wrongScore;

    public event Action OnAnswerSelected; // Event for answer selection
    public event Action OnWrongAnswer; // Event for wrong answer

    private void Awake()
    {
        originalQna = new List<QuestionAndAnswer_KTX_B3>(Qna);

        // Implement the singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();
            }
            else
            {
                // Khi hết thời gian
                timeRemaining = 0;
                isTimerRunning = false;
                GameOver();
               // RestartGame();
            }
        }
    }

    private void Start()
    {
        TotalQuestion = Qna.Count;
        //  GoPanel.SetActive(false);
      //  isTimerRunning = true; // Kích hoạt bộ đếm thời gian
        GeneratedQuestion();
    }

    void SetAnswers()
    {
        // Create a list of indices for answers
        List<int> answerIndices = new List<int>();
        for (int i = 0; i < Qna[CurrentQuestions].Answers.Length; i++)
        {
            answerIndices.Add(i);
        }

        // Shuffle the list of answer indices
        for (int i = 0; i < answerIndices.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, answerIndices.Count);
            int temp = answerIndices[i];
            answerIndices[i] = answerIndices[randomIndex];
            answerIndices[randomIndex] = temp;
        }

        // Assign shuffled answers to the buttons
        for (int i = 0; i < Options.Length; i++)
        {
            Options[i].GetComponent<Answers_KTX_B3>().isCorrect = false;
            Options[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Qna[CurrentQuestions].Answers[answerIndices[i]];

            // Set the correct answer based on the shuffled index
            if (Qna[CurrentQuestions].CorrectAnswer == answerIndices[i] + 1)
            {
                Options[i].GetComponent<Answers_KTX_B3>().isCorrect = true;
            }
        }
    }

    void GeneratedQuestion()
    {
        if (Qna.Count > 0)
        {
            CurrentQuestions = UnityEngine.Random.Range(0, Qna.Count);

            QuestionTxt.text = Qna[CurrentQuestions].Question;
            SetAnswers();
        }
        else
        {
            Debug.Log("Out of Questions");
            GameOver();
        }
    }

    public void AnswerSelected(bool isCorrect, GameObject button)
    {
        StartCoroutine(AnswerFeedback(isCorrect, button));
    }

    IEnumerator AnswerFeedback(bool isCorrect, GameObject button)
    {
        if (isCorrect)
        {
            button.GetComponent<Image>().color = Color.green;
            score++;
        }
        else
        {
            button.GetComponent<Image>().color = Color.red;
            wrongScore++;

            // Trigger the event for wrong answer
            OnWrongAnswer?.Invoke();
        }

        yield return new WaitForSeconds(1);

        button.GetComponent<Image>().color = Color.white; // Reset màu nút

        if (wrongScore >= 3)
        {
            GameOver();
           
        }
        else
        {
            Qna.RemoveAt(CurrentQuestions);
            GeneratedQuestion();

            // Trigger the event when an answer is selected
            OnAnswerSelected?.Invoke();
        }
    }

    public void GameOver()
    {
        QuizPanel.SetActive(false);
        GoPanel.SetActive(true);

        if (score <= 2)
        {
            ScoreTxt.text = "Thật đáng tiếc, bạn chỉ trả lời đúng " + score + "/" + 8 + ". Hãy thử lại vào lần tới";
        }
        else if (score > 2 && score <= 5)
        {
            ScoreTxt.text = "Bạn đã trả lời đúng " + score + "/" + 8 + ". Hãy cố gắng vào lần tới";
        }
        else if (score > 5)
        {
            ScoreTxt.text = "Xin chúc mừng. Bạn đã trả lời đúng " + score + "/" + 8 + ". Hãy tiếp tục phát huy";
        }
    }

    public int GetScore()
    {
        return score;
    }

    public int GetWrongScore()
    {
        return wrongScore;
    }

    private void UpdateTimerDisplay()
    {
        // Tính toán phút và giây còn lại
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void RestartGame()
    {
        // Reset các biến cần thiết
        score = 0;
        wrongScore = 0;

        // Kiểm tra xem danh sách Qna có đủ câu hỏi để reset không
        if (Qna.Count < TotalQuestion)
        {
            // Nếu số lượng câu hỏi đã bị giảm, cần reload hoặc reset lại danh sách câu hỏi ban đầu
            ResetQuestionList(); // Giả sử bạn có một hàm để reset danh sách câu hỏi ban đầu
        }

        // Reset lại thời gian và bắt đầu lại trò chơi
        timeRemaining = 300f;
       // isTimerRunning = true;

        // Hiển thị lại QuizPanel và ẩn GoPanel
     //   QuizPanel.SetActive(true);
     //   GoPanel.SetActive(false);

        GeneratedQuestion();
    }

    private void ResetQuestionList()
    {
        Qna = new List<QuestionAndAnswer_KTX_B3>(originalQna);
    }

}
