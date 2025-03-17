using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using Unity.VisualScripting;
namespace PS
{
    public class DetectPlayer : Conditional
    {
       
        public Enemy enemy;
        public RangeEnemy rangeEnemy;
        public float detectRadius = 3f;
        public float roarRange = 6f;


        public override TaskStatus OnUpdate()
        {
           
            Collider[] hitColiders;
            if (enemy != null)
            {
                hitColiders= Physics.OverlapSphere(enemy.transform.position, detectRadius);
            }
            else
            {
                hitColiders= Physics.OverlapSphere(rangeEnemy.transform.position, detectRadius);
            }

            foreach (Collider _hitColider in hitColiders)
            {
                if (_hitColider.gameObject.CompareTag("Player"))
                {
                   
                    if (rangeEnemy != null)
                    {
                        rangeEnemy.transform.LookAt(GameManager.Instance.PlayerTransform.position);
                        rangeEnemy.LookAtIK.solver.target = GameManager.Instance.CameraFollowTransform;
                        rangeEnemy.LookAtIK.solver.Update();
                        rangeEnemy.AimIK.solver.target = GameManager.Instance.CameraFollowTransform;
                        rangeEnemy.AimIK.solver.Update();
                        rangeEnemy.aimTarget = _hitColider.transform;
                        rangeEnemy.transform.LookAt(GameManager.Instance.PlayerTransform.position);
                        rangeEnemy.RangeEnemyAnimationManager.Roar();
                        rangeEnemy.RoarAlert(roarRange);
                    }
                  
                    if (enemy != null)
                    {
                        enemy.transform.LookAt(GameManager.Instance.PlayerTransform.position);
                        enemy.EnemyAnimationManger.Roar();
                        enemy.RoarAlert(roarRange);
                    }
               
                    Debug.Log("Ã£À½");
                
                        
                    return TaskStatus.Success;
                }
            }
            return TaskStatus.Failure;
        }

    }
}
