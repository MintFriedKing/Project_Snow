//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using static Player;

//namespace PS
//{
//    public partial class Player : MonoBehaviour
//    {
//        #region private Members
//        private bool isPersistentDamage;
//        private bool isRepeating;
//        private bool isCanBeHit;
//        private PlayerState playerState;
//        private PlayerHealth playerHealth;
//        #endregion
//        private void Awake()
//        {
//            isPersistentDamage = false;
//            isCanBeHit = true;
//            playerHealth = this.GetComponent<PlayerHealth>();
//        }

//        #region 충돌 관련 이벤트

//        private void OnCollisionEnter(Collision collision)
//        {

//        }
//        private void OnTriggerEnter(Collider other)
//        {
//                if (isCanBeHit == true &&playerState != PlayerState.DIE && other.tag == "Weapon")
//                {
//                    isCanBeHit = false;
//                    EnemyWeapon enemyWeapon = other.gameObject.GetComponent<EnemyWeapon>();
//                    enemyWeapon.HitSphereCollider.enabled = false;
//                    float damage = enemyWeapon.Damage;
//                    playerHealth.TakeDamage(damage, other.gameObject.transform.forward);
//                    Debug.Log("맞았음");
//                }
            
          
//        }
//        #endregion
//    }
//}
