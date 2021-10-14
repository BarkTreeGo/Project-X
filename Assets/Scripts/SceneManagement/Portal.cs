using RPG.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.SceneManagement;
using GameDevTV.Saving;


namespace RPG.SceneManagement
{
        
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D
        }
        
        [SerializeField] int sceneToLoad = 1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 1.5f;
        [SerializeField] float fadeInTime = 1.5f;
        [SerializeField] float fadeWaitTime = 0.5f;

        // cached References
        SavingWrapper savingWrapper;

        private void Start()
        {
            savingWrapper = FindObjectOfType<SavingWrapper>();
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set.");
                yield break;
            }

            DontDestroyOnLoad(gameObject); //only works if a gameobject is at the root of the scene

            Fader fader = FindObjectOfType<Fader>();

            PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;

            yield return fader.FadeOut(fadeOutTime);

            //for the player its unique identifier should be the same between scenes
            //save current level
            savingWrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad); //waits until the scene has loaded

            PlayerController newPlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;

            //load current level
            savingWrapper.Load();

            Portal otherPortal = GetOtherPortal(destination);
            UpdatePlayer(otherPortal);

            savingWrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);

            newPlayerController.enabled = true;
        }

        private Portal GetOtherPortal(DestinationIdentifier destination)
        {
            Portal[] portals = FindObjectsOfType<Portal>();

            foreach (Portal portal in portals)
            {
                if (portal == this) { continue; }
                if (portal.destination != destination) { continue; }
                return portal;
            }
            return null;
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            NavMeshAgent playerNavMesh = player.GetComponent<NavMeshAgent>();
            playerNavMesh.enabled = false;
            playerNavMesh.Warp(otherPortal.spawnPoint.position);            
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            playerNavMesh.enabled = true;
        }
    }

}
