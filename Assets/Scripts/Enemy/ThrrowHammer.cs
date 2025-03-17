using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class ThrrowHammer : Action
    {
        public Boss boss;
        public float minDistnace;
        public float maxDistance;
        public override void OnStart()
        {
            
        }
        public override TaskStatus OnUpdate()
        {
            if (boss.CurrentDistance>minDistnace&& boss.CurrentDistance <= maxDistance)
            {
                boss.BossAnimationManger.ThrrowHammer();
                StartCoroutine(boss.BossVFXSpawnAbilities.ThrrowHammer());
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }
        }
    }
}
