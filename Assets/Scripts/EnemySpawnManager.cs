using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace PS
{
    [Serializable]
    public class SpawnInfomation
    {
        public Transform spawnTransfrom;
        public GameObject spawnObject;
        
    }
    public class EnemySpawnManager : MonoBehaviour
    {
        public bool isSpawn;
        public List<SpawnInfomation> spawnInfomations;
       
        private void OnTriggerEnter(Collider other)
        {
            if (isSpawn == false && other.gameObject.tag=="Player")
            {
               isSpawn = true;
                foreach (SpawnInfomation spawnInfomation in spawnInfomations)
                {
                  GameObject Enemy = Instantiate(spawnInfomation.spawnObject,spawnInfomation.spawnTransfrom.position, spawnInfomation.spawnObject.transform.rotation);
                  
                }
               Debug.Log("플레이어 충돌");
                      
            }       
        }       
    }
}
