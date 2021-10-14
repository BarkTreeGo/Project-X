using UnityEngine;
using GameDevTV.Saving;
using RPG.Stats;
using RPG.Core;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        LazyValue<float> healthPoints;
        [SerializeField] float regenerationPercentage = 70f;
        [SerializeField] TakeDamageEvent takeDamage;
        public UnityEvent OnDie;
        
        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float> //required to take variable input at runtime
        {
        }        
        
        bool wasDeadLastFrame = false;

        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start()
        {
            healthPoints.ForceInit();
            
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth; //no parenthesis since we are not calling the level, but merely subscribing to it a method 
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;  
        }

        public bool IsDead()
        {
            return healthPoints.value <= 0;
        }        

        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoints.value = Mathf.Max(0, healthPoints.value - damage);
            
            if (IsDead())
            {
                OnDie.Invoke();                
                AwardExperience(instigator);            
            }
            else
            {
                takeDamage.Invoke(damage);
            }
            UpdateState();

        }

        public void Heal(float healthToRestore)
        {            
            healthPoints.value = Mathf.Min(healthPoints.value + healthToRestore, GetMaxhealthPoints());
            UpdateState();

        }

        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GethealthPoints()
        {
            return healthPoints.value;
        }

        public float GetMaxhealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health); 
        }        

        private void UpdateState()
        {
            Animator animator = GetComponent<Animator>();
            if (!wasDeadLastFrame && IsDead())
            {
                animator.SetTrigger("die");
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }

            if (wasDeadLastFrame && !IsDead())
            {
                animator.Rebind(); //resets the animator
            }

            wasDeadLastFrame = IsDead();
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) { return; }
            
            float experiencePoints = GetComponent<BaseStats>().GetStat(Stat.ExperienceReward);
            instigator.GetComponent<Experience>().GainExperience(experiencePoints);
            
        }

        private void RegenerateHealth()
        {
            float regenerateHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
            healthPoints.value = Mathf.Max(healthPoints.value, regenerateHealthPoints);
        }

        public object CaptureState()
        {
            return healthPoints.value; //basic types like int, float string are by default serializable
        }
        public void RestoreState(object state)
        {
            healthPoints.value = (float)state; //casting to convert object to float
            UpdateState();            
        }

        
    }
}

