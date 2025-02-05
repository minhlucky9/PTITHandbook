using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    [CreateAssetMenu(menuName = "Scriptable Objects/NPC/Conservation Data")]
    public class NPCConservationSO : ScriptableObject
    {
        public string conservationId;
        public NPCState npcState;
        public string questId;
        public string stepId;

        public bool startFromFirstDialog;
        public List<DialogConservation> dialogs;

        public DialogConservation GetDialog(string id)
        {
            int index = GetDialogIndex(id);
            if (index != -1)
            {
                return dialogs[index];
            } else
            {
                return null;
            }
        }

        public int GetDialogIndex(string id)
        {
            for (int i = 0; i < dialogs.Count; i ++)
            {
                if (dialogs[i].dialogId.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            conservationId = npcState + (questId != "" ? ("_" + questId) : "")  + (stepId != ""? ("_" + stepId) : "");
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

    
    [System.Serializable]
    public class DialogConservation
    {
        public string dialogId;
        [TextArea(5, 10)]
        public string message;
        public AudioClip voice;
        public List<DialogResponse> possibleResponses;

        //editor
        [HideInInspector] public Vector2 nodePosition;

        public DialogConservation()
        {
            possibleResponses = new List<DialogResponse>();
            dialogId = "";
            message = "";
            nodePosition = new Vector2(10, 10);
        }
    }

    [System.Serializable]
    public class DialogResponse
    {
        [TextArea(1, 2)]
        public string message;
        public Sprite icon;
        public string nextDialogId;
        public DialogExecuteFunction executedFunction;

        public DialogResponse()
        {
            message = "";
            icon = null;
            nextDialogId = "";
            executedFunction = DialogExecuteFunction.None;
        }
    }

    public enum DialogExecuteFunction
    {
        None,
        //Interaction handle
        StopInteract,
        
        //Quest step handle - NPC Controller
        FinishQuestStep,
        StartQuestMinigame,
        FinishQuestStepThenStartMinigame,
        OnQuestMinigameFail,
        OnQuestMinigameSuccess,

        //Quiz handle - Quiz Manager
        NextQuiz,
        AnswerCorrect,
        AnswerWrong,

        //Functional Window - NPC Controller
        OpenShopFunctionalWindow
    }
}
