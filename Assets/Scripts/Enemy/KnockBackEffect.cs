using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class KnockBackEffect : MonoBehaviour
    {
        public float knockBackPower = 10f;
        public BoxCollider boxCollider;

        private void OnDisable()
        {
            //boxCollider.enabled = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                boxCollider.enabled = false;
                Vector3 direction = (other.transform.position - transform.position).normalized;
                other.GetComponent<Player>().PlayerRigidbody.AddForce(direction * knockBackPower ,ForceMode.Impulse);
                Debug.Log("Player È÷Æ®");
            }
        }
    }
}
