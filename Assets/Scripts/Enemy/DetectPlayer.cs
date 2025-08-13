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

        public override void OnStart()
        {
            if (rangeEnemy != null)
            {
                rangeEnemy.RangeEnemyAnimationManager.Animator.SetLayerWeight(1,1f);
            }
        }

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
                        //LookAtik
                        rangeEnemy.transform.LookAt(GameManager.Instance.PlayerInputManager.transform.position); //발견시 처다보고
                        rangeEnemy.LookAtIK.solver.target = GameManager.Instance.CameraFollowTransform; //플레이어를 처다보며
                        rangeEnemy.LookAtIK.solver.Update();
                        //AimIk
                        rangeEnemy.AimIK.solver.target = GameManager.Instance.CameraFollowTransform; // 플레이어를 타켓으로 설정한다.
                        rangeEnemy.AimIK.solver.Update();   // 솔버설정을 업데이트 
                        rangeEnemy.aimTarget = _hitColider.transform; //마찬가지로 에임을 결정한다.
                        rangeEnemy.transform.LookAt(GameManager.Instance.PlayerTransform.position); //플레이어 방향으로 축을 튼다.
                        //FullbodyIk
                        rangeEnemy.FullBodyBipedIK.solver.leftHandEffector.positionWeight = 0f;
                        rangeEnemy.RangeEnemyAnimationManager.Roar();  //울부짖는 애니메이션
                        rangeEnemy.RoarAlert(roarRange);  // 동료를 부른다.
                    }
                  
                    if (enemy != null)
                    {
                        enemy.transform.LookAt(GameManager.Instance.PlayerTransform.position);
                        enemy.EnemyAnimationManger.Roar();
                        enemy.RoarAlert(roarRange);
                    }
                                              
                    return TaskStatus.Success;
                }
            }
            return TaskStatus.Failure;
        }

    }
}
