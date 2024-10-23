using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Answers_A1_2 : MonoBehaviour
{

    public bool isCorrect = false;
    public QuizA1_2 quizManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Answer()
    {
        if (isCorrect)
        {
            Debug.Log("Correct Answer");
            //  quizManager.Correct();
            quizManager.AnswerSelected(true, gameObject);
        }
        else
        {
            Debug.Log("Wrong Answer");
            //   quizManager.Wrong();
            quizManager.AnswerSelected(false, gameObject);
        }
    }
}
