using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public class Health : MonoBehaviour
{
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
    private void Init()
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
        enemy.enemyState = EnemyState.Die;
        enemy.EnemyAnimationManger.Die();
        float animationTime = enemy.EnemyAnimationManger.Animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(enemy.gameObject, animationTime); 
       
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

   

}
