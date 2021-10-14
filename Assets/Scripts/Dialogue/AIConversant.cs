using RPG.Attributes;
using RPG.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] Dialogue AIdialogue = null;
        [SerializeField] string conversantName;

        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public string GetName()
        {
            return conversantName;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (AIdialogue == null)
            {
                return false;
            }


            Health health = GetComponent<Health>();
            if (health && health.IsDead()) 
            { 
                return false; 
            }

            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<PlayerConversant>().StartDialogue(this, AIdialogue);
            }

            return true;
        }
    }
}
