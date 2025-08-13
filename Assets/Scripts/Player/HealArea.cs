using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace PS
{
    public class HealArea : MonoBehaviour
    {
        public GameObject healAuraPrefab;
        public List<GameObject> healAuras;
        public List<GameObject> players;
        public float healRate; //Æ½ ·¹ÀÌÆ® 
        public float healPower; //Æ½´ç Èú 
        private Dictionary<GameObject ,Coroutine> activeHealOverTime = new Dictionary<GameObject, Coroutine>(); // --- Èú

        public GameObject nerfAuraPrefab;
        public List<GameObject> nerfAuras;
        public List<GameObject> enemys;
        public float damageRate;
        public float damage;
        private Dictionary<GameObject, Coroutine> activeDealOverTime = new Dictionary<GameObject, Coroutine>();

        private void OnDisable()
        {
            
    
            foreach (GameObject healAura in healAuras)
            {
                //players.Clear();
                //healAuras.Clear();
                Destroy(healAura);          
            }
            foreach (GameObject dealAura in nerfAuras)
            {
                //nerfAuras.Clear();
                //enemys.Clear();
                Destroy(dealAura);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {     
                GameObject heal = Instantiate(healAuraPrefab);
                heal.transform.position = other.transform.position;
                healAuras.Add(heal);
                players.Add(other.transform.gameObject); 

                if (activeHealOverTime.ContainsKey(other.transform.gameObject) == false) 
                {
                    Coroutine healCoroutine = StartCoroutine(HealofTime(other.transform.gameObject));
                    activeHealOverTime.Add(other.transform.gameObject ,healCoroutine); 
                }
               
            }
            if (other.gameObject.CompareTag("Enemy"))
            { 
                GameObject nerf = Instantiate(nerfAuraPrefab);
                nerf.transform.position = other.transform.position;
                nerfAuras.Add(nerf);
                enemys.Add(other.transform.gameObject);
                if (activeDealOverTime.ContainsKey(other.transform.gameObject) == false)
                {
                    Coroutine dealCoroutine = StartCoroutine(DealOfTime(other.transform.gameObject));
                    activeDealOverTime.Add(other.transform.gameObject, dealCoroutine);
                }
         
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].transform.gameObject == other.transform.gameObject)
                    {
                        healAuras[i].transform.position = other.transform.position;
                       
                    }
                }          
            }

            if (other.gameObject.CompareTag("Enemy"))
            {
                for (int i = 0; i < enemys.Count; i++)
                {
                    if (enemys[i].transform.gameObject == other.transform.gameObject)
                    {
                        nerfAuras[i].transform.position = other.transform.position;

                    }
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if(other.gameObject.CompareTag("Player"))
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].transform.gameObject == other.transform.gameObject)
                    {
                        Destroy(healAuras[i].transform.gameObject);
                        healAuras.RemoveAt(i);
                         
                    }
                }    
                if (activeHealOverTime.ContainsKey(other.transform.gameObject) ==true)
                {
                    StopCoroutine(activeHealOverTime[other.transform.gameObject]);
                    activeHealOverTime.Remove(other.transform.gameObject);
                }
            }
            if (activeDealOverTime.ContainsKey(other.transform.gameObject) == true)
            {
                StopCoroutine(activeDealOverTime[other.transform.gameObject]);
                activeDealOverTime.Remove(other.transform.gameObject);
            }

        }

        private IEnumerator HealofTime(GameObject gameObject)
        {         
            while (true)
            {
                PlayerHealth playerHealth =GameManager.Instance.CurrentPlayer.PlayerHealth;
                if (playerHealth != null)
                {
                    playerHealth.CurrentHealth += healPower;
                    UIManager.Instance.UpdateCharacterList();
                
                }
                yield return new WaitForSeconds(healRate);
            }
        }

        private IEnumerator DealOfTime(GameObject gameObject)
        { 
            Health health = gameObject.GetComponent<Health>();
            while (true)
            {
                if (health != null)
                {
                    health.TakeDamage(damage, Vector3.zero);
                }
                yield return new WaitForSeconds(healRate);
            }
        }
    }
}
