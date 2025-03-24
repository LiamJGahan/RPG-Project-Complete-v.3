using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System;
using RPG.Attributes;
using GameDevTV.Utils;
using UnityEngine.AI;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] float aggroCoolDownTime = 5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 2f;
        [Range(0f,1f)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        [SerializeField] float shoutDistance = 4f;


        Fighter fighter;
        GameObject player;
        Health health;
        Mover mover;

        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceAggevated = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;

        void Awake()
        {
            fighter = GetComponent<Fighter>();
            player = GameObject.FindGameObjectWithTag("Player");
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();

            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
            guardPosition.ForceInit();
        }

        Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        void Update()
        {
            if(health.IsDead()) { return; }

            if(IsAggrevated() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if(timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        public void Reset()
        {
            GetComponent<NavMeshAgent>().Warp(guardPosition.value);

            timeSinceLastSawPlayer = Mathf.Infinity;
            timeSinceAggevated = Mathf.Infinity;
            timeSinceArrivedAtWaypoint = Mathf.Infinity;

            currentWaypointIndex = 0;
        }

        public void Aggrevate()
        {
            timeSinceAggevated = 0f;
        }

        void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggevated += Time.deltaTime;
        }

        void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);

            foreach(RaycastHit hit in hits)
            {
                AIController aI = hit.collider.GetComponent<AIController>();
                if(aI == null) { continue; }

                aI.Aggrevate();
            }
        }

        void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;

            if(patrolPath != null)
            {
                if(AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }

                nextPosition = GetCurrentWaypoint();
            }

            if(timeSinceArrivedAtWaypoint >= waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        bool AtWaypoint()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToPlayer < waypointTolerance;
        }

        void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        bool IsAggrevated()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

            return distanceToPlayer < chaseDistance || timeSinceAggevated < aggroCoolDownTime;
        }

        //Called by unity
        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}