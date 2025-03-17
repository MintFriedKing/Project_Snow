using BehaviorDesigner.Runtime.Tasks;
using PS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForAnimationTime : Wait
{
    public Enemy enemy;
    public RangeEnemy rangeEnemy;
    //기다리는 시간
    private float animationWaitDuration;
    //이 노드가 시작한 시간을 알아야 런타임 중 무리없이 작동할수 있다.
    private float startTime;
    public override void OnStart()
    {
        startTime = Time.time;
        float curretTime;
        if (enemy != null)
        {
            curretTime = enemy.EnemyAnimationManger.Animator.GetCurrentAnimatorStateInfo(0).length;
        }
        else
        {
            
            curretTime = rangeEnemy.RangeEnemyAnimationManager.Animator.GetCurrentAnimatorStateInfo(0).length;
        }
        waitTime = curretTime;
        animationWaitDuration = waitTime.Value;
   
    }
    public override TaskStatus OnUpdate()
    {
        if(startTime + animationWaitDuration <Time.time)
        {
            Debug.Log("애니메이션 웨이팅 성공");
            if (rangeEnemy != null)
            {
                rangeEnemy.RangeEnemyAnimationManager.Animator.SetBool("IsMove",true);
            }
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
        
    }
}
