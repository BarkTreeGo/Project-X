using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Utils;
using GameDevTV.Inventories;
using GameDevTV.Saving;

namespace RPG.Combat
{    
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float timeBetweenAttacks = 1f;               
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
        [SerializeField] bool IsAutoAttacking = false;
        [SerializeField] float autoAttackRange = 4f;
        
        float timeSinceLastAttack = Mathf.Infinity;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        // cached References
        Health target;        
        Mover mover;
        Animator animator;
        Equipment equipment;

        private void Awake()
        {
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            equipment = GetComponent<Equipment>();
            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }

        private void Start()
        {
            currentWeapon.ForceInit();          
        }              

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);            
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;            

            if (target == null) { return; }
            if (target.IsDead()) 
            {
                if (IsAutoAttacking)
                {
                    target = FindNewTargetInRange();
                    if (target == null) { return; }
                }
                else
                {
                    return;
                }
            }

            if (!GetIsInRange(target.transform)) 
            {
                mover.MoveTo(target.transform.position, 1f);
            }
            else
            {
                mover.Cancel();                
                AttackBehavior();                
            }
        }

        private Health FindNewTargetInRange()
        {
            Health best = null;
            float bestDistance = Mathf.Infinity;
            foreach (var candidate in FindAllTargetsInRange())
            {
                float candidateDistance = Vector3.Distance(transform.position, candidate.transform.position);
                if (candidateDistance < bestDistance)
                {
                    best = candidate;
                    bestDistance = candidateDistance;
                }
            }
            return best;
        }

        private IEnumerable<Health> FindAllTargetsInRange()
        {
            RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, autoAttackRange, Vector3.up, 0);

            foreach (RaycastHit hit in raycastHits)
            {
                Health health = hit.transform.GetComponent<Health>();
                if (health == null) { continue; }
                if (health.IsDead()) { continue; }
                if (health.gameObject == gameObject) { continue; }
                yield return health;
            }
        }

        private void AttackBehavior()        
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                //this will trigger the Hit() event
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private void UpdateWeapon()
        {
            var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if (weapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
            else
            {
                EquipWeapon(weapon);
            }
        }
        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        private void TriggerAttack()
        {
            animator.ResetTrigger("stopAttack");
            animator.SetTrigger("attack");
        }

        public Health GetTarget()
        {
            return target;
        }

        //animation event
        private void Hit()
        {            
            if (target == null) { return; }

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            BaseStats targetBaseStats = target.GetComponent<BaseStats>();
           
            if (targetBaseStats != null)
            {
                float defence = targetBaseStats.GetStat(Stat.Defence);
                damage /= 1 + (defence / damage);
            }            

            if (currentWeapon.value != null) //only triggers for e.g. bow and swords as they have a weapon prefab
            {
                currentWeapon.value.OnHit();
            }

            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {             
                target.TakeDamage(gameObject, damage);
            }            
        }

        //animation event
        private void Shoot()
        {
            Hit();
        }

        private bool GetIsInRange(Transform targetTransform) 
        {
            return (Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.GetWeaponRange());            
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; } //in the PC RayCast first determine is we have a combat target (e.g. and not a terrain)
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) &&
                !GetIsInRange(combatTarget.transform)) //enables shoting at an enemy even if you have no path to it (e.g. shooting over water).
            { 
                return false; 
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead(); //in the PC RayCast test if it is still alive and has a Health component
        }
        
        public void Attack(GameObject combatTarget)
        {
            target = combatTarget.GetComponent<Health>();
            GetComponent<ActionScheduler>().StartAction(this);
        }

        public void Cancel()
        {
            target = null;
            StopAttack();
            GetComponent<Mover>().Cancel();
        }

        public Transform GetHandTransform(bool isRightHand)
        {
            if (isRightHand)
            {
                return rightHandTransform;
            }
            else
            {
                return leftHandTransform;
            }
        }

        private void StopAttack()
        {
            animator.SetTrigger("stopAttack");
            animator.ResetTrigger("attack");
        }       
    }
}

