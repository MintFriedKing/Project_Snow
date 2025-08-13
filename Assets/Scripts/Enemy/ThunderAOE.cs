using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class ThunderAOE : Action
    {
        public Boss boss;
        public int attackNumber;
        public float minDistance;
        public float maxDistance;
     
        public override TaskStatus OnUpdate()
        {
            if (boss.CurrentDistance > minDistance && boss.CurrentDistance <= maxDistance)
            {     
                boss.BossAnimationManger.ThunderAOE(attackNumber);
                StartCoroutine(boss.BossVFXSpawnAbilities.ThunderAOE());        
                return TaskStatus.Success;   
            }
            else 
            {
                return TaskStatus.Failure;
            }
        }
    }
}
