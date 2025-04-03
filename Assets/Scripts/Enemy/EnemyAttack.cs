using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using PS;

public class EnemyAttack : BehaviorDesigner.Runtime.Tasks.Action
{
    public float distance;
    public Enemy enemy;
    public RangeEnemy rangeEnemy;
   
    public override void OnStart()
    {
       
      
    }
    public override TaskStatus OnUpdate()
    {
        if (CheckDistance() == true)
        {
            if (enemy != null)
            {
                enemy.NavMeshAgent.velocity = Vector3.zero;
                enemy.NavMeshAgent.isStopped = true;
                enemy.Attack();
                enemy.isHeardRoar = false;
                Debug.Log("°ø°ÝÇÔ");
            }
            else
            {
                rangeEnemy.isHeardRoar = false;
                rangeEnemy.NavMeshAgent.velocity = Vector3.zero;
                rangeEnemy.NavMeshAgent.isStopped = true;
                Vector3 direction = (GameManager.Instance.CameraFollowTransform.position -rangeEnemy.transform.position).normalized;
                rangeEnemy.transform.rotation = Quaternion.Slerp(rangeEnemy.transform.rotation,Quaternion.LookRotation(direction),Time.deltaTime *2f);              
                rangeEnemy.LookAtIK.solver.target = GameManager.Instance.CameraFollowTransform;
                rangeEnemy.LookAtIK.solver.Update();
                rangeEnemy.AimIK.solver.target = GameManager.Instance.CameraFollowTransform;
                rangeEnemy.AimIK.solver.IKPositionWeight = 1f;
                rangeEnemy.AimIK.solver.Update();
                rangeEnemy.FullBodyBipedIK.solver.leftHandEffector.positionWeight = 1f;
                rangeEnemy.FullBodyBipedIK.solver.Update();
                rangeEnemy.RangeEnemyAnimationManager.Movement(0f);
                rangeEnemy.RangeEnemyAnimationManager.Animator.SetLayerWeight(1,1f);
                rangeEnemy.Attack();
                
            }
            return TaskStatus.Running;
        }
        else 
        {
            if (enemy != null)
            {
                enemy.NavMeshAgent.isStopped = false;
            }
            else
            {
                rangeEnemy.NavMeshAgent.isStopped = false;
            }
            return TaskStatus.Success;
        }
       
    }
    public bool CheckDistance()
    {
        float currentdistance;
        if (enemy != null)
        {
            currentdistance = Vector3.Distance(GameManager.Instance.PlayerTransform.position, enemy.transform.position);
        }
        else
        {
            currentdistance = Vector3.Distance(GameManager.Instance.PlayerTransform.position, rangeEnemy.transform.position);
        }
        if (currentdistance <= distance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
