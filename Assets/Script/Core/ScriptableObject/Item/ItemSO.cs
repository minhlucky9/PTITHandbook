using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    public class ItemSO : ScriptableObject
    {
        public string itemId;
        public ShopCategorySO category;
        public GameObject prefabs;
        public bool isStackable;
        public int maxStackSize;

        public virtual void UseItem() { }

        private void OnValidate()
        {
#if UNITY_EDITOR
            itemId = this.name;
#endif
        }
    }
}

