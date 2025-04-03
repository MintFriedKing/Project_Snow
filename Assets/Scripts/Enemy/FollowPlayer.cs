using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using Unity.VisualScripting;
using PS;
using RootMotion.FinalIK;
using UnityEngine.SocialPlatforms;

public class FollowPlayer : Action
{
    public Enemy enemy;
    public RangeEnemy rangeEnemy;
    public float followspeed = 2f;
    public float distance = 1f;

    
    public override TaskStatus OnUpdate()
    {
       

        if (CheckDistance() == true)
        {
            return TaskStatus.Success;
        }
        else if (enemy != null && enemy.enemyState == Enemy.EnemyState.Die ||rangeEnemy != null &&rangeEnemy.enemyState == Enemy.EnemyState.Die)
        {
                enemy.IsMove = false;
                return TaskStatus.Failure;
                      
        }
        Follow();     
        return TaskStatus.Running;
    }
    public void Follow()
    {
        //Vector3 direction = (player.transform.position - enemy.transform.position).normalized;
        //Rigidbody rigidbody = enemy.MoveRigidBody;
        //rigidbody.MovePosition(enemy.transform.position + direction * speed * Time.deltaTime);
        if (enemy != null)
        {
            enemy.IsMove = true;
            enemy.EnemyAnimationManger.Movement(true);
            enemy.NavMeshAgent.SetDestination(GameManager.Instance.PlayerInputManager.transform.position);
            Debug.Log(GameManager.Instance.PlayerInputManager.transform.position);
            enemy.NavMeshAgent.speed = followspeed;
        }
        else if (rangeEnemy != null)
        {
            //rangeEnemy.NavMeshAgent.velocity = Vector3.zero;
            rangeEnemy.NavMeshAgent.isStopped = false;
            rangeEnemy.transform.LookAt(GameManager.Instance.PlayerInputManager.transform.position);
            rangeEnemy.LookAtIK.solver.target = GameManager.Instance.CameraFollowTransform;
            rangeEnemy.LookAtIK.solver.Update();
            rangeEnemy.AimIK.solver.IKPositionWeight = 1f;
            rangeEnemy.AimIK.solver.target = GameManager.Instance.CameraFollowTransform;
            rangeEnemy.AimIK.solver.Update();
            rangeEnemy.FullBodyBipedIK.solver.leftHandEffector.positionWeight = 1f;
            rangeEnemy.FullBodyBipedIK.solver.Update();
            rangeEnemy.RangeEnemyAnimationManager.Animator.SetLayerWeight(1, 1f);
            rangeEnemy.RangeEnemyAnimationManager.Movement(1);
            rangeEnemy.NavMeshAgent.SetDestination(GameManager.Instance.PlayerTransform.position);
            rangeEnemy.NavMeshAgent.speed = followspeed;
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
