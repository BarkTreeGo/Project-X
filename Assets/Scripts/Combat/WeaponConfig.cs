using UnityEngine;
using RPG.Attributes;
using GameDevTV.Inventories;
using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order =0)]
    public class WeaponConfig: EquipableItem, IModifierProvider
    {
        [SerializeField] Weapon weaponPrefab = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] float weaponDamage = 40f;
        [SerializeField] float weaponPercentageBonus = 0f;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            Weapon myWeapon = null;

            Transform handTransform;
            handTransform = GetTransform(rightHand, leftHand);

            if (weaponPrefab != null)
            {
                myWeapon = Instantiate(weaponPrefab, handTransform); //instatiates the weapon as a child of the handtransform parent
                myWeapon.gameObject.name = weaponName;                
            }
            
            //can you cast the current controller (may be default or an override) as the default Animator Override?             
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null) //if it is the default one do nothing
            {
                //if the current override is not the default one set it to its parent (i.e. default) of the overrride controller
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController; //finds the parent animator controller in the overrideControler           
            }

            return myWeapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName); //finds a child of the parent by name
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }

            if (oldWeapon == null) { return; }

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded)
            {
                handTransform = rightHand;
            }
            else
            {
                handTransform = leftHand;
            }

            return handTransform;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        public float GetWeaponDamage()
        {
            return weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return weaponPercentageBonus;
        }

        public float GetWeaponRange()
        {
            return weaponRange;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return weaponDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return weaponPercentageBonus;
            }
        }
    }

}



