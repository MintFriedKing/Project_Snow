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
        animator.SetTrigger("Attack");
    }
    public virtual void Die()
    {
        animator.SetTrigger("Die");
    }
}
