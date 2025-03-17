using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace PS
{
    public class EnemyRoar : Conditional
    {
        public Transform playerTransform;
        public Enemy enemy;
        public RangeEnemy rangeEnemy;
        public float roarRange;
        public override void OnStart()
        {
            playerTransform = GameManager.Instance.PlayerTransform;
            if (rangeEnemy != null)
            {
                rangeEnemy.aimTarget = playerTransform;
            }
        }
        public override TaskStatus OnUpdate()
        {
            if (enemy != null)
            {
                if (enemy.isHeardRoar == true)
                {
                    enemy.EnemyAnimationManger.Roar();
                    enemy.RoarAlert(roarRange);
                    return TaskStatus.Success;
                }
                else
                {
                    enemy.isHeardRoar = false;
                    return TaskStatus.Failure;
                }
            }
            else 
            {
                if (rangeEnemy.isHeardRoar == true)
                {
                    rangeEnemy.RangeEnemyAnimationManager.Roar();
                    rangeEnemy.RoarAlert(roarRange);
                    return TaskStatus.Success;
                }
                else
                {
                    rangeEnemy.isHeardRoar = false;
                    return TaskStatus.Failure;
                }
            }
              
        }
        
    }
}
