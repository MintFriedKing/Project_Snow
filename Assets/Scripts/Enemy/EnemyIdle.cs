using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class EnemyIdle : Action
    {
        public Enemy enemy;
        public RangeEnemy rangeEnemy;
      
        public override TaskStatus OnUpdate()
        {
            if (enemy != null)
            {
               
                enemy.EnemyAnimationManger.Movement(false);
                return TaskStatus.Success;
            }
            else 
            {
                rangeEnemy.AimIK.solver.target = null;
                rangeEnemy.AimIK.solver.IKPositionWeight = 0;
                rangeEnemy.AimIK.solver.Update();
                rangeEnemy.LookAtIK.solver.target = null;
                rangeEnemy.LookAtIK.solver.Update();
                rangeEnemy.FullBodyBipedIK.solver.leftHandEffector.positionWeight = 0;
                rangeEnemy.RangeEnemyAnimationManager.Movement(0f);
                rangeEnemy.RangeEnemyAnimationManager.Animator.SetLayerWeight(1,0f);
                return TaskStatus.Success;
            }
        }


        
    }
 

}
