using Cinemachine;
using RPG.Attributes;
using RPG.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class Respawner : MonoBehaviour
    {
        [SerializeField] Transform respawnLocation;
        [SerializeField] float respawnDelay = 3f;
        [SerializeField] float fadeTime = 0.2f;
        [SerializeField] float healthRegenPercentage = 20f;
        [SerializeField] float enemyHealthRegenPercentage = 20f;
        private void Awake()
        {
            GetComponent<Health>().OnDie.AddListener(Respawn);            
        }

        private void Start()
        {
            if (GetComponent<Health>().IsDead())
            {
                Respawn();
            }
        }

        private void Respawn()
        {
            StartCoroutine(RespawnRoutine());            
        }

        private IEnumerator RespawnRoutine()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();
            yield return new WaitForSeconds(respawnDelay);
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeTime);
            RespawnPlayer();
            ResetEnemies();
            savingWrapper.Save();
            yield return fader.FadeIn(fadeTime);
        }

        private void ResetEnemies()
        {
            foreach (AIController enemyController in FindObjectsOfType<AIController>())
            {
                Health health = enemyController.GetComponent<Health>();               

                if (health && !health.IsDead())
                {
                    enemyController.Reset();
                    health.Heal(health.GetMaxhealthPoints() * (enemyHealthRegenPercentage / 100));
                }
            }
        }

        private void RespawnPlayer()
        {
            Vector3 positionDelta = respawnLocation.position - transform.position;
            GetComponent<NavMeshAgent>().Warp(respawnLocation.position);
            Health health = GetComponent<Health>();
            health.Heal(health.GetMaxhealthPoints() * (healthRegenPercentage / 100));

            ICinemachineCamera activeVirtualCamera = FindObjectOfType<CinemachineBrain>().ActiveVirtualCamera;
            if (activeVirtualCamera.Follow == transform)
            {
                activeVirtualCamera.OnTargetObjectWarped(transform, positionDelta); //needs to 'warn'virtual camera when moving large distances
            }
        }
    }
}
