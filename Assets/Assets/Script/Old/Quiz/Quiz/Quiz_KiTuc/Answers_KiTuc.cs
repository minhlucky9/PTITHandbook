using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Answers_KiTuc : MonoBehaviour
{

    public bool isCorrect = false;
    public Quiz_KiTuc quizManager_KiTuc;
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
            quizManager_KiTuc.AnswerSelected(true, gameObject);
        }
        else
        {
            Debug.Log("Wrong Answer");
            //   quizManager.Wrong();
            quizManager_KiTuc.AnswerSelected(false, gameObject);
        }
    }
}
