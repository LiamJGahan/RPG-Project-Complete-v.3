using Cinemachine;
using RPG.Attributes;
using RPG.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace RPG.Control
{
    class Respawner : MonoBehaviour
    {
        [SerializeField] Transform respawnLocation;
        [SerializeField] float respawnDelay = 3;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float healthRegenPercentage = 20;
        [SerializeField] float enemyHealthRegenPercentage = 20;

        void Awake()
        {
            GetComponent<Health>().onDie.AddListener(Respawn);
        }

        void Start()
        {
            if (GetComponent<Health>().IsDead())
            {
                Respawn();
            }
        }

        void Respawn()
        {
            StartCoroutine(RespawnRoutine());
        }

        IEnumerator RespawnRoutine()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();

            yield return new WaitForSeconds(respawnDelay);
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            RespawnPlayer();
            ResetEnemies();

            savingWrapper.Save();

            yield return fader.FadeIn(fadeInTime);
        }

        void RespawnPlayer()
        {
            Vector3 postitionDelta = respawnLocation.position - transform.position;

            GetComponent<NavMeshAgent>().Warp(respawnLocation.position);
            Health health = GetComponent<Health>();
            health.Heal(health.GetMaxHealthPoints() * healthRegenPercentage / 100);

            ICinemachineCamera activeVirtualCamera = FindObjectOfType<CinemachineBrain>().ActiveVirtualCamera;

            if(activeVirtualCamera.Follow == transform)
            {
                activeVirtualCamera.OnTargetObjectWarped(transform, postitionDelta);
            }
        }

        void ResetEnemies()
        {
            foreach(AIController enemyController in FindObjectsOfType<AIController>())
            {
                Health health = enemyController.GetComponent<Health>();

                if (health && !health.IsDead())
                {
                    enemyController.Reset();
                    health.Heal(health.GetMaxHealthPoints() * enemyHealthRegenPercentage / 100);
                }
            }
        }
    }
}
