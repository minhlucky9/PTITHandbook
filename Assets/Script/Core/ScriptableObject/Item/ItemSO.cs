using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    public class ItemSO : ScriptableObject
    {
        public string itemId;
        public GameObject prefabs;

        private void OnValidate()
        {
#if UNITY_EDITOR
            itemId = this.name;
#endif
        }
    }
}

