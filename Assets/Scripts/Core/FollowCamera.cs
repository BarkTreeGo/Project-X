using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Transform target;

        void LateUpdate()
        {
            transform.position = target.position; //Main Camera will be childed under this
                                                  //the follow camera object will move with the player
        }
    }
}

