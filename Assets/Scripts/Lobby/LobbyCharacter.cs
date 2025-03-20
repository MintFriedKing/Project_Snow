using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class LobbyCharacter : MonoBehaviour
    {
        public GameObject spawnEffect;
        public void OnEnable()
        {
          
        }
        public void CreateSpawnParticle()
        {
            GameObject SpawnParticle = Instantiate(spawnEffect);
            SpawnParticle.transform.position = this.transform.position;
            SpawnParticle.SetActive(true);
        }
    }
}
