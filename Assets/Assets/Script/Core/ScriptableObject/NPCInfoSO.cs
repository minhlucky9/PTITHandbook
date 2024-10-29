using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    [CreateAssetMenu(menuName = "Scriptable Objects/NPC/NPC Info")]
    public class NPCInfoSO : ScriptableObject
    {
        public string npcId;
        public string npcName;
        public string npcRole;
        public NPCConservationSO normalConservation;

        private void OnValidate()
        {
#if UNITY_EDITOR
            npcId = this.name;
#endif
        }
    }

    public enum NPCState
    {
        NORMAL,
        HAVE_QUEST,
        IN_PROGRESS,
        QUEST_COMPLETE,
        COUNTDOWN
    }
}

