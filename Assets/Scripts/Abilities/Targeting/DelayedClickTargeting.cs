using RPG.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Delayed Click Targeting", menuName = "Abilities/Targeting/Delayed Click", order = 0)]
    public class DelayedClickTargeting : TargetingStrategy
    {
        [SerializeField] Texture2D cursorTexture;
        [SerializeField] Vector2 cursorHotspot;
        [SerializeField] LayerMask layerMask;
        [SerializeField] float areaEffectRadius = 5f;
        [SerializeField] Transform targetingPrefab;

        Transform targetingPrefabInstance = null;

        public override void StartTargeting(AbilityData data, Action finished)
        {
            //need to get hold of a monobehavior to run a CoRoutine
            PlayerController playerController = data.GetUser().GetComponent<PlayerController>();
            playerController.StartCoroutine(Targeting(data, playerController, finished));
        }

        //only get's executed when the finished function is done, which returns (passes in) the GameObject array.
        private IEnumerator Targeting(AbilityData data, PlayerController playerController, Action finished)
        {            
            playerController.enabled = false;
            if (targetingPrefabInstance == null)
            {
                targetingPrefabInstance = Instantiate(targetingPrefab);
            }
            else
            {
                targetingPrefabInstance.gameObject.SetActive(true);
            }
            targetingPrefabInstance.localScale = new Vector3(areaEffectRadius*2, 1, areaEffectRadius*2);

            while (!data.IsCancelled())
            {
                Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
                RaycastHit raycastHit;
                if (Physics.Raycast(PlayerController.GetMouseRay(), out raycastHit, 1000, layerMask))
                {
                    targetingPrefabInstance.position = raycastHit.point;
                    if (Input.GetMouseButtonDown(0))
                    {
                        //absorb the whole mouseclick until it is released otherwise player will move prematurely                   
                        yield return new WaitWhile(() => Input.GetMouseButton(0));                        
                        data.SetTargetedPoint(raycastHit.point);
                        data.SetTargets(GetGameObjectsInRadius(raycastHit.point));                                             
                        break;
                    }
                }
                //Run every frame
                yield return null;
            }
            targetingPrefabInstance.gameObject.SetActive(false);
            playerController.enabled = true;
            finished();
        }

        private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point)
        {                  
            RaycastHit[] hits = Physics.SphereCastAll(point, areaEffectRadius, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                yield return hit.collider.gameObject;
            }                  
        }
    }
}
