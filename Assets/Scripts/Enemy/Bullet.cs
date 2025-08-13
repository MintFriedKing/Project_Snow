using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PS.Player;

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

            if (SkillManager.Instance.isHasShiled == true && other.gameObject.CompareTag("Shield"))
            {
                UIManager.Instance.ShieldBar.TakeDamage(damage);
            }
            //bool isCheck = playerState != PlayerState.DIE &&
            //   other.tag == "Bullet";

            if (other.CompareTag("Player"))
            {

                Player player = other.gameObject.GetComponent<Player>()
                                    ?? other.gameObject.GetComponentInParent<Player>()
                                    ?? other.gameObject.GetComponentInChildren<Player>();
                if (player != null)
                {
                    player.PlayerHealth.TakeDamage(damage, other.gameObject.transform.forward);
                    Debug.Log("¸Â¾ÒÀ½");
                }
             
            }

            Destroy(this.gameObject);

        }
    }
}
