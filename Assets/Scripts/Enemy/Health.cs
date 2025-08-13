using BehaviorDesigner.Runtime;
using PS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public class Health : MonoBehaviour
{
    [SerializeField]
    private Enemy enemy;
    private RagDoll ragDoll;
    [SerializeField]
    protected float maxhealth;
    [SerializeField]
    protected float currentHealth;
    [SerializeField]
    protected HealthBar healthBar;
    public RagDoll RagDoll { get { return ragDoll; } set { ragDoll = value; }}
    public HealthBar HealthBar { get { return healthBar;} set { healthBar = value; } }
    public float Maxhealth { get { return maxhealth; } set { maxhealth = value;} }
    public float CurrentHealth { get { return currentHealth; } set { currentHealth = value; } }

    private void Awake()
    {
        Init();
    }
    protected virtual void Init()
    {
        currentHealth = maxhealth;
        enemy = this.GetComponent<Enemy>();
        if (healthBar == null)
        {
            healthBar = this.GetComponentInChildren<HealthBar>();
        }
       
    }

    public virtual void Die()
    {
        //ragDoll.EnableRagdoll(); //레그돌 발동 
        BehaviorTree behaivortree = this.GetComponent<BehaviorTree>();
        behaivortree.enabled = false;
        enemy.NavMeshAgent.isStopped = true;
        enemy.enemyState = EnemyState.Die;
        if (enemy.EnemyAnimationManger != null)
        {
            enemy.EnemyAnimationManger.Die();
        }
        else
        {
            RangeEnemy rangeEnemy = this.GetComponent<RangeEnemy>();
            rangeEnemy.FullBodyBipedIK.solver.leftHandEffector.positionWeight = 0f;
            rangeEnemy.AimIK.solver.IKPositionWeight = 0f;
            enemy.GetComponent<RangeEnemyAnimationManager>().Die();
        }
       
    }
    public virtual void TakeDamage(float _amount ,Vector3 _direction)
    {
        if (enemy.enemyState == EnemyState.Idle)
        {
            enemy.enemyState = EnemyState.Combat;
        }

        currentHealth -= _amount;

        healthBar.SetHealthBar(currentHealth / maxhealth);
        if (currentHealth <= 0.0f)
        {
            Die();
        }
        enemy.BlinkTimer = enemy.blinkDuration;
    }
    //private IEnumerator DestoryObjectRoutine()
    //{
    //    enemy.EnemyAnimationManger.Die();
        
    //    AnimatorStateInfo stateInfo = enemy.EnemyAnimationManger.Animator.GetCurrentAnimatorStateInfo(0);

    //    while (!enemy.EnemyAnimationManger.Animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
    //    {
    //        yield return null;  // 다음 프레임까지 대기
    //    }

    //    while (stateInfo.normalizedTime < 1.0f)
    //    {
    //        yield return null;
    //    }
    //    Destroy(enemy.gameObject);
    //}

    private void DestoryObject()
    { 
        Destroy(enemy.gameObject);
    }

}
