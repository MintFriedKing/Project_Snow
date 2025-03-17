using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class Bullet : MonoBehaviour
    {
        public float damage;
        private void Start()
        {
            Destroy(this.gameObject ,5f); 
        }
        private void OnTriggerEnter(Collider other)
        {

            if (SkillManager.Instance.IsShield == true && other.gameObject.CompareTag("Shield"))
            {
                UIManager.Instance.ShieldBar.TakeDamage(damage);
            }        
            Destroy(this.gameObject);

        }
    }
}
