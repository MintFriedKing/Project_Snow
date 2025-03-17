using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class RangeEnemyAnimationManager : EnemyAnimationManger
    {

       
        public override void Roar()
        {
            base.Roar();
        }
        public void Movement(float _speed)
        { 
            animator.SetFloat("Speed",_speed);
            //Debug.Log("이동 애니메이션 디버그");
        }
        public override void Attack()
        {
            Debug.Log("공격 애니메이션 디버그");
        }
    }
}
