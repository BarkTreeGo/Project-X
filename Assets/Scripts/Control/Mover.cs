using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using GameDevTV.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        // initialization paramterers
        [SerializeField] float maxSpeed = 6f;
        [SerializeField] float maxNavMeshPathLenght = 25f;

        // cached references
        NavMeshAgent navMeshAgent;
        Animator animator;
        Health health;
        
        // Start is called before the first frame update
        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            health = GetComponent<Health>();           
             
        }

        // Update is called once per frame
        void Update()
        {
            navMeshAgent.enabled = !health.IsDead();
            UpdateAnimator();
            
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);            
            MoveTo(destination, speedFraction);
        }
        
        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) { return false; }
            if (path.status != NavMeshPathStatus.PathComplete) { return false; }            
            if (GetPathLenght(path) > maxNavMeshPathLenght) { return false; }
            
            return true;
        }


        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.SetDestination(destination);
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity; //as based on the worlds orietation
            Vector3 localVelocity = transform.InverseTransformDirection(velocity); //as based from the player's perspective and orientation
            float speed = localVelocity.z;
            animator.SetFloat("forwardSpeed", speed);
        }
        private float GetPathLenght(NavMeshPath path)
        {
            Vector3[] pathCorners = path.corners;
            float total = 0;

            if (path.corners.Length < 2) { return 0; }

            for (int i = 0; i < pathCorners.Length - 1; i++) //i goes to the length -1
            {
                total += Vector3.Distance(pathCorners[i], pathCorners[i + 1]);
            }
            return total;
        }

        //Interfaces
        public void Cancel()
        {
            navMeshAgent.isStopped = true;
            //Fighter.cancel is called via the ActionScheduler and IAction interface to reduce circular dependency
        }

        [System.Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }

        //object is a root class and can be anything
        //For the Capture State it needs to be marked as Serializable
        public object CaptureState()
        {
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.eulerAngles);
            return data;
            
            /* 
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);
            return data;
            */
        }

        //Restore is activated between Awake and Start
        public void RestoreState(object state)
        {
            MoverSaveData data = (MoverSaveData)state;

            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = data.position.ToVector();
            transform.eulerAngles = data.rotation.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;

            /*
            Dictionary<string, object> data = (Dictionary<string, object>)state; //casting to convert object to a Dictionary
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = ((SerializableVector3)data["position"]).ToVector();
            transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
            */
        }
    }
}


