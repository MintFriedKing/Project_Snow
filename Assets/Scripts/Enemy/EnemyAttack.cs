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
    private float nextFireCheckTime;
    private float fireRate;
    public override void OnStart()
    {
        if (rangeEnemy != null)
        {
            rangeEnemy.RangeEnemyAnimationManager.Animator.SetLayerWeight(1, 1f);
            rangeEnemy.gunLaser.gameObject.SetActive(true);
            nextFireCheckTime = 0f;
            fireRate = rangeEnemy.fireRate;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (CheckDistance() == true)
        {
            if (enemy != null)
            {
                enemy.NavMeshAgent.velocity = Vector3.zero;
                enemy.NavMeshAgent.isStopped = true;      //네비 끄고
                enemy.IsMove = false;                     // 움직임 끄고      
                transform.LookAt(Player.Instance.transform); //공격시작 
                enemy.AttackAnimationStart();
                enemy.isHeardRoar = false;
                Debug.Log("공격함");
            }
            else
            {
                //이동관련
                rangeEnemy.isHeardRoar = false;
                rangeEnemy.NavMeshAgent.velocity = Vector3.zero;
                rangeEnemy.NavMeshAgent.isStopped = true;
                rangeEnemy.RangeEnemyAnimationManager.Movement(0f);
                //자신의 바라보는 방향 
                Vector3 direction = (GameManager.Instance.CameraFollowTransform.position -rangeEnemy.transform.position).normalized;
                rangeEnemy.transform.rotation = Quaternion.Slerp(rangeEnemy.transform.rotation,Quaternion.LookRotation(direction),Time.deltaTime *2f);              
                rangeEnemy.LookAtIK.solver.target = GameManager.Instance.CameraFollowTransform;
                rangeEnemy.LookAtIK.solver.Update();
                //에임
                rangeEnemy.AimIK.solver.target = GameManager.Instance.CameraFollowTransform;
                rangeEnemy.AimIK.solver.IKPositionWeight = 1f;
                rangeEnemy.AimIK.solver.Update();
                rangeEnemy.FullBodyBipedIK.solver.leftHandEffector.positionWeight = 1f;
                rangeEnemy.FullBodyBipedIK.solver.Update();
                if (Time.time >= nextFireCheckTime )
                {
                    nextFireCheckTime = Time.time +fireRate;
                    rangeEnemy.isAttack = true;
                }
                else
                {
                    rangeEnemy.isAttack = false;
                }
                rangeEnemy.Attack();             
            }   
        }
        if(CheckDistance() == false)
        {
            if (enemy != null)
            {
                enemy.NavMeshAgent.isStopped = false;
              
            }
            else
            {
                rangeEnemy.NavMeshAgent.isStopped = false;
                rangeEnemy.gunLaser.gameObject.SetActive(false);
            }
         
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
    public bool CheckDistance()
    {
        float currentdistance;
        if (enemy != null)
        {
            currentdistance = Vector3.Distance(GameManager.Instance.PlayerInputManager.transform.position, enemy.transform.position);
            
        }
        else
        {
            currentdistance = Vector3.Distance(GameManager.Instance.PlayerInputManager.transform.position, rangeEnemy.transform.position);
        }

        if (currentdistance <= distance)
        {
            Debug.Log(distance);
            return true;
        }
        else
        {
            Debug.Log(distance);
            return false;
        }
    }

}
