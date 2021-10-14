using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RPG.Attributes;

namespace RPG.Combat
{    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 2f;
        [SerializeField] bool isHoming = true;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] GameObject[] destroyOnHit = null; //for parts of the projectile which are immediatly destroyed (e.g. head)
        [SerializeField] float lifeAfterImpact = 2f; //for parts of the projectile which may linger a bit (e.g. trail)
        
        float MaxLifeTime = 10f;
        GameObject instigator = null;
        Health target = null;
        Vector3 targetPoint;
        float damage = 0f;

        [SerializeField] UnityEvent OnHit;


        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if (target != null && isHoming && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());                
            }
            
            transform.Translate(Vector3.forward * speed * Time.deltaTime);            
        }
        private Vector3 GetAimLocation()
        {
            if (target == null)
            {
                return targetPoint;
            }

            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return target.transform.position;
            }
            
            return target.transform.position + Vector3.up*(targetCapsule.height/2);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            SetTarget(instigator, damage, target);
        }

        public void SetTarget(Vector3 targetPoint, GameObject instigator, float damage)
        {
            SetTarget(instigator, damage, null, targetPoint);
        }

        public void SetTarget(GameObject instigator, float damage, Health target = null, Vector3 targetPoint = default)//overload where some parameters are optional
        {
            this.target = target;
            this.targetPoint = targetPoint;
            this.damage = damage;
            this.instigator = instigator;

            Destroy(gameObject, MaxLifeTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            Health health = other.GetComponent<Health>();
            if (target != null && health != target) return;                         
            if (health == null || health.IsDead()) return;
            if (other.gameObject == instigator) return;

            health.TakeDamage(instigator, damage);
            speed = 0;

            OnHit.Invoke();

            if (hitEffect!= null)
            {
                GameObject ImpactEffect = Instantiate(hitEffect, GetAimLocation(), transform.rotation);
                //hiteffects are destroyed via a separate 'destroy after effect' script to make it more generic
            }

            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy); //parts of projectile which are immediatly destroyed
            }

            Destroy(gameObject, lifeAfterImpact);           
            
        }

    }
}


