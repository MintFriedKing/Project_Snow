using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class CheckDistance : Conditional
    {
        public Boss boss;
        public float minCheckValue;
        public float maxCheckValue; //최대설정거리

        public override TaskStatus OnUpdate()
        {
            if (boss.CurrentDistance > minCheckValue  && boss.CurrentDistance <= maxCheckValue && 
                boss.BossVFXSpawnAbilities.attacking  == false
                )
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }
           
        }
    }
}
