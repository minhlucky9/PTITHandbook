using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interaction
{
    public class ConservationManager : MonoBehaviour
    {
        public static ConservationManager instance;
        [Header("NPC Message")]
        public UIAnimationController messageContainer;
        public TMP_Text npcMessage;
        public TMP_Text npcName;
        public TMP_Text npcRole;

        [Header("Player Response")]
        public UIAnimationController responseController;
        public GameObject responsePrefab;
        public Sprite defautIcon;

        GameObject targetNPC;
        NPCConservationSO conservationData;

        private void Awake()
        {
            instance = this;
        }

        public void InitConservation(GameObject npc, NPCConservationSO conservation)
        {
            targetNPC = npc;
            conservationData = conservation;

            //update name and role text
            NPCController npcController = npc.GetComponent<NPCController>();
            if (npcController)
            {
                npcName.text = npcController.npcInfo.npcName;
                npcRole.text = npcController.npcInfo.npcRole;
            }
            

            //setup first conservation
            DialogConservation firstDialog;
            if(conservationData.startFromFirstDialog)
            {
                firstDialog = conservation.dialogs[0];
            } else
            {
                //can get a random dialog
                firstDialog = conservation.dialogs[0];
            }
            SetupDialogInConservation(firstDialog);
        }

        public void ChangeTargetNPC(GameObject npc) { targetNPC = npc; }

        void SetupDialogInConservation(DialogConservation dialog)
        {
            //Setup npc message
            npcMessage.text = dialog.message;
   
            //Setup player response
            //Clear old responses in container
            foreach (Transform child in responseController.transform)
            {
                child.gameObject.SetActive(false);
            }

            //Add new response
            for(int i = 0; i < dialog.possibleResponses.Count; i++)
            {
                DialogResponse responseData = dialog.possibleResponses[i];

                //find or create response object
                GameObject responseObject;
                if (i < responseController.transform.childCount)
                {
                    responseObject = responseController.transform.GetChild(i).gameObject;
                } else
                {
                    responseObject = Instantiate(responsePrefab, responseController.transform);
                }

                //setup response data
                responseObject.GetComponentInChildren<TMP_Text>().text = responseData.message;
                responseObject.transform.GetChild(1).GetComponent<Image>().sprite = responseData.icon != null? responseData.icon : defautIcon;
                responseObject.GetComponent<Button>().onClick.AddListener(delegate () 
                {
                    if(responseData.nextDialogId != "") 
                    {
                        DialogConservation nextDialog = conservationData.GetDialog(responseData.nextDialogId);
                        if(nextDialog != null) { StartCoroutine(UpdateConservation(nextDialog)); }
                    } 
                    if (responseData.executedFunction != DialogExecuteFunction.None) { targetNPC.SendMessage(responseData.executedFunction.ToString()); }
                });

                responseObject.SetActive(true);
            }
            responseController.UpdateObjectChange();
        }

        void ClearAllButtonEvent()
        {
            Button[] btns = responseController.GetComponentsInChildren<Button>();
            foreach(Button b in btns)
            {
                b.onClick.RemoveAllListeners();
            }
            //
            DisableAllButton();
        }

        void EnableAllButton()
        {
            Button[] btns = responseController.GetComponentsInChildren<Button>();
            foreach (Button b in btns)
            {
                b.interactable = true;
            }
        }

        void DisableAllButton()
        {
            Button[] btns = responseController.GetComponentsInChildren<Button>();
            foreach (Button b in btns)
            {
                b.interactable = false;
            }
        }
        public IEnumerator UpdateConservation(DialogConservation nextDialog)
        {
            ClearAllButtonEvent();
            responseController.Deactivate();
            
            yield return new WaitForSeconds(0.4f);
            messageContainer.Deactivate();
            
            yield return new WaitForSeconds(0.6f);
            SetupDialogInConservation(nextDialog);
            messageContainer.Activate();

            yield return new WaitForSeconds(0.2f);
            responseController.Activate();
            EnableAllButton();
            yield return null;
        }

        public IEnumerator ActivateConservationDialog()
        {
            messageContainer.Activate();
            yield return new WaitForSeconds(0.3f);
            responseController.Activate();
            EnableAllButton();
        }

        public IEnumerator DeactivateConservationDialog()
        {
            ClearAllButtonEvent();
            responseController.Deactivate();
            yield return new WaitForSeconds(0.3f);
            messageContainer.Deactivate();
        }
    }
}

