using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Item/Shop Category")]
    public class ShopCategorySO : ScriptableObject
    {
        public string shopCategoryId;
        public Sprite iconInShop;
        private void OnValidate()
        {
#if UNITY_EDITOR
            shopCategoryId = this.name;
#endif
        }
    }
}
