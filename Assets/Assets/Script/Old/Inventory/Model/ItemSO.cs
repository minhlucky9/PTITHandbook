using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class ItemSO : ScriptableObject
    {
        [field: SerializeField]
        public bool IsStackable { get; set; }

        public int ID => GetInstanceID();

        [field: SerializeField]
        public int MaxStackSize { get; set; } = 1;

        [field: SerializeField]
        public string Name { get; set; }

        [field: SerializeField]
        [field: TextArea]
        public string Description { get; set; }

        [field: SerializeField]
        public Sprite ItemImage { get; set; }

        [field: SerializeField]
        public float HealthIncrease { get; set; } // Existing property

        [field: SerializeField]
        public float EffectDuration { get; set; }  // Duration of the effect

        [field: SerializeField]
        public float SpeedMultiplier { get; set; }  // Multiplier for speed boost

        [field: SerializeField]
        public string Tags { get; set; } // List of tags to define item functionalities

        // Example tags: "Health", "Mana", "Speed", "ManaLock", "NonConsumable"
    }

}


/*
        [field: SerializeField]
        public List<ItemParameter> DefaultParametersList { get; set; }

    }

    [Serializable]
    public struct ItemParameter : IEquatable<ItemParameter>
    {
       public ItemParameterSO itemParameter;
        public float value;

        public bool Equals(ItemParameter other)
        {
            return other.itemParameter == itemParameter;
        }
*/


