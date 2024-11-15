using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Interaction {
    [CreateAssetMenu(menuName = "Scriptable Objects/Minigame/Quiz Conservation Data")]
    public class QuizConservationSO : ScriptableObject
    {
        public string conservationId;
        public string questId;
        public string stepId;
        public int numberOfQuestion;
        public int numberOfMaxWrong;
        public List<QuestionAndAnswer> qnas;
        public DialogConservation quizIntro;
        private void OnValidate()
        {
#if UNITY_EDITOR
            conservationId = questId + "_" + stepId;
#endif
        }
    }

    [System.Serializable]
    public class QuestionAndAnswer
    {
        [TextArea(2,4)]
        public string Question;
        public string[] Answers;
        public int CorrectAnswer;
        [TextArea(2, 4)]
        public string Explaination;
    }
}

