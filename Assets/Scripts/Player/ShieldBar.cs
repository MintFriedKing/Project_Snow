using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class ShieldBar : HealthBar
    {
        public float currentShield;
        public float maxShield;
        protected override void LateUpdate()
        {
           
        }
        public void Init()
        {
            currentShield = maxShield;
        }
        public override void SetHealthBar(float _percentage)
        {
            base.SetHealthBar(_percentage);
        }
        public void TakeDamage(float _amount)
        {
            currentShield -= _amount;
            SetHealthBar(currentShield / maxShield);
            if (currentShield <= 0.0f)
            {
                //this.gameObject.SetActive(false);
                SkillManager.Instance.IsShield = false;
            }      
        }

    }
}
