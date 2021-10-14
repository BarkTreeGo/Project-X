using UnityEngine;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using GameDevTV.Utils;
using System;
using UnityEngine.AI;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        //initialization parameters
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 3f;
        [SerializeField] float agroCooldownTime = 5f;
        [SerializeField] float shoutDistance = 5f;

        [Range(0,1)] [SerializeField] float patrolSpeedFraction = 0.2f;

        //cached references
        Mover mover;
        Fighter fighter;
        GameObject player;
        Health health;
        ActionScheduler actionScheduler;

        //States
        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoiny = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;
        int currentWaypointIndex = 0;

        private void Awake()
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            player = GameObject.FindWithTag("Player");
            health = GetComponent<Health>();
            actionScheduler = GetComponent<ActionScheduler>();
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
            guardPosition.ForceInit();
        }

        public void Reset()
        {
            NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.Warp(guardPosition.value);
            timeSinceLastSawPlayer = Mathf.Infinity;
            timeSinceArrivedAtWaypoiny = Mathf.Infinity;
            timeSinceAggrevated = Mathf.Infinity;
            currentWaypointIndex = 0;
        }

        private void Start()        {

            guardPosition.ForceInit();
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Update()
        {
            if (health.IsDead()) { return; }

            if (IsAggrevated() && fighter.CanAttack(player))
            {                
                AttackBehavior();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehavior();
            }
            else if (timeSinceLastSawPlayer >= suspicionTime)
            {
                PatrolBehavior();
            }

            UpDateTimers();
        }

        public void Aggrevate()
        {
            timeSinceAggrevated = 0;
        }

        private void UpDateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoiny += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private void AttackBehavior()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);

            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);

            foreach (RaycastHit hit in hits)
            {
                AIController ai = hit.collider.GetComponent<AIController>();
                if (ai == null) { continue; }

                ai.Aggrevate();
            }              
        }

        private void SuspicionBehavior()
        {
            actionScheduler.CancelCurrentAction();            
        }

        private void PatrolBehavior()
        {
            Vector3 nextPosition = guardPosition.value;            

            if (patrolPath != null)
            {
                if (AtWayPoint())
                {                    
                    CycleWayPoint();
                    timeSinceArrivedAtWaypoiny = 0;                    
                }
                nextPosition = GetCurrentWayPoint();
            }

            if (timeSinceArrivedAtWaypoiny > waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        private bool AtWayPoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWayPoint());
            return (distanceToWaypoint < waypointTolerance);
        }

        private Vector3 GetCurrentWayPoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWayPoint()
        {
            currentWaypointIndex =  patrolPath.GetNextIndex(currentWaypointIndex);            
        }       

        private bool IsAggrevated()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return ((distanceToPlayer < chaseDistance) || (timeSinceAggrevated < agroCooldownTime));            
        }

        //called by unity itself
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
        
    }
}

