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
        animationWaitDuration = waitTime.Value; //애니메이션 시간 
   
    }
    public override TaskStatus OnUpdate()
    {
        if (startTime + animationWaitDuration < Time.time)
        {
            Debug.Log("애니메이션 웨이팅 성공");
            if (rangeEnemy != null)  //원거리 적일경우에는 
            {
                rangeEnemy.RangeEnemyAnimationManager.Movement(0f);
            }
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
        
    }
}
