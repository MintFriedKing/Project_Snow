using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityDebug;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PS
{
    public class DrawTelegraph : Action
    {
        public Boss boss;
        public float minCheckValue;
        public float maxCheckValue; //최대설정거리
        public float drawTime;
        public int drawNumber;
        public override void OnStart()
        {
            
            StartCoroutine(boss.SkillDelay(drawTime, null));    
        }
        public override TaskStatus OnUpdate()
        {
            if (boss.CurrentDistance <= maxCheckValue)
            {
                
                if (boss.isCoolTime == true)
                {
                    boss.transform.position = new Vector3(boss.transform.position.x, 0, boss.transform.position.z);
                    boss.DrawTelegraph(drawNumber);                    
                }
                else if (boss.isCoolTime == false)
                {
                    boss.EraseTelegraph();
                    return TaskStatus.Success;
                }
                return TaskStatus.Running; 

            }
            else
            {
               
                    Debug.Log("드로우중 예외 생김");
                    return TaskStatus.Failure;
                
            }
        }
    }
}
