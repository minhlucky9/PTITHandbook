using PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Interactable : MonoBehaviour
    {
        public float radius = 0.6f;
        public string interactableText;
        public bool isInteracting = false;
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, radius);
        }

        public virtual void Interact()
        {
      
        }

        public virtual void StopInteract()
        {
    
        }
    }
}

