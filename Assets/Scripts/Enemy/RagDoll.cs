using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDoll : MonoBehaviour
{ 
    [SerializeField]
    private Rigidbody[] rigidBodys;
    private EnemyAnimationManger enemyAnimationManger;
    private void Awake()
    {
        Init();
    }
    private void Start()
    {
        DisableRagDoll();
    }
    private void Init()
    {
        rigidBodys = GetComponentsInChildren<Rigidbody>();
        enemyAnimationManger =this.GetComponent<EnemyAnimationManger>();
    
    }
    public void DisableRagDoll()
    {
        foreach (var rigidbody in rigidBodys)
        {
            rigidbody.isKinematic = true;
        }
        enemyAnimationManger.Animator.enabled = true;
    }
    public void EnableRagdoll()
    {
        foreach (var rigidbody in rigidBodys)
        {
            rigidbody.isKinematic = false;
        }
        enemyAnimationManger.Animator.enabled = false;
    }
}
