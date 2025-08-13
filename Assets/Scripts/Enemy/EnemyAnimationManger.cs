using BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationManger : MonoBehaviour
{
    [SerializeField]
    protected Animator animator;   
    public Animator Animator { get { return animator; } set { animator = value; } }
   
    public virtual void Roar()
    {
        animator.SetTrigger("Roar");
    }
    public virtual void Movement(bool _isMove)
    {
        animator.SetBool("IsMove", _isMove);
    }
    public virtual void Attack()
    {
        animator.ResetTrigger("Attack");  // 트리거 초기화
        animator.SetTrigger("Attack");
    }
    public virtual void Die()
    {
        animator.SetTrigger("Die");
    }
    public virtual void Distance(float _distance)
    {
        animator.SetFloat("Distance", _distance);
    }
}
