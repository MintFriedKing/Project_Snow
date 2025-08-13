using BehaviorDesigner.Runtime;
using PS;
using RootMotion.FinalIK;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;

[Serializable]
public class EnemyInfo
{
    public List<SkinnedMeshRenderer> skinnedMeshRenderers;
    public List<Material> spawnMaterial;
    public List<Material> basicMaterial;
    public List<MeshRenderer> weaponMeshRenderers;
    public List<Material> weaponbasicMaterial;
    public List<Material> weaponSpawnMaterial;
}
public class Enemy : MonoBehaviour
{
    //[SerializeField, Header("이동용 리지드바디")]
    //private Rigidbody moveRigidBody;
    [SerializeField, Header("데미지")]
    private float damage;
    [SerializeField, Header("왼손 히트콜라이더")]
    private SphereCollider leftHendColider;
    [SerializeField, Header("오른손 히트콜라이더")]
    private SphereCollider rightHendColider;
    [SerializeField, Tooltip("레그돌들")]
    protected Rigidbody[] rigidBodys;
    [SerializeField]
    protected Health health;
    [SerializeField]
    protected HitBox hitBox;
    [SerializeField]
    protected RagDoll ragDoll;
    [SerializeField]
    protected EnemyAnimationManger enemyAnimationManger;
    protected SkinnedMeshRenderer skinnedMeshRenderer;
    private float blinkTimer;
    protected NavMeshAgent navMeshAgent;
    [SerializeField]
    protected bool isMove;
    [SerializeField]
    public bool isAttack;
    protected bool isCombat;
    [SerializeField,Header("공격속도")]
    private float attackBetweenTime;
    private float attackTimer;
    private float roarRange;
    #region 스폰(Phase) 관련 전역 protected 변수 
    [SerializeField]
    protected float startMaterialSpawnValue; //스폰 시작 값
    [SerializeField]
    protected float endMaterialSpawnValue;  // 엔드 값 
    [SerializeField]
    protected float spawnTime;
    #endregion
    protected GameObject recallVFX;
    [SerializeField]
    protected GameObject recallParticle;
    [SerializeField]
    protected float recallParticlesDestroyTime =2f;
    [SerializeField]
    protected float recallVFXTime = 2f;
    protected GameObject spawnVFX;
    [SerializeField]
    protected GameObject spawnPaticle;
    [SerializeField]
    protected float spawnParticlesDestroyTime;
    [SerializeField]
    protected float spawnVFXTime = 2f;
    [SerializeField]
    public EnemyInfo enemyInfo;

    public float attackRadius;
    public EnemyAnimationManger EnemyAnimationManger { get { return enemyAnimationManger; } set { enemyAnimationManger = value; } }
    public NavMeshAgent NavMeshAgent { get { return navMeshAgent; } set { navMeshAgent = value; } }
    public float Damage { get { return damage; } set { damage = value; } }
    public bool IsMove { get { return isMove; } set { isMove = value; } }
    #region 하울링 기능 전역 멤버 변수 
    public float RoarRange { get { return roarRange; }set { roarRange = value; } }
    public LayerMask enemyLayer;
    public bool isHeardRoar = false;
    #endregion
    #region 히트 이펙트 관련 전역 멤버 변수
    public float BlinkTimer { get { return blinkTimer; } set { blinkTimer =value; } }
    public float blinkIntensity; //마테리얼을 이용해 색을 바꿔 타격처리 해볼라고 ㅇㅇ 반짝이는 정도
    public float blinkDuration; // 지속시간
    public Color materialColor;
    #endregion
    public bool IsCombat { get { return isCombat; } set { isCombat = value; } }
    public float AttackBetweenTime { get { return attackBetweenTime; } set { attackBetweenTime = value; } }
    public float AttackTimer { get { return attackTimer; } set { attackTimer = value; } }
    public enum EnemyState
    { 
        Idle,
        Combat,
        Die
    }
    public EnemyState enemyState;
    private void OnEnable()
    {
        if (enemyInfo.weaponMeshRenderers != null)
        { 
            ChangeWeaponMaterial(enemyInfo.weaponSpawnMaterial);
        }
        ChangeSkinMeshMaterial(enemyInfo.spawnMaterial);
        StartCoroutine(Recall());
       

    }
    private void Awake()
    {
        Init();
    }
    private void Update()
    {
        ShowHitImpact();

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackBetweenTime)
        {
            isAttack = true;
            attackTimer = 0;
        }

    }
    protected virtual void Init()
    {
        health = this.GetComponent<Health>();
        ragDoll = this.GetComponent<RagDoll>();
        hitBox = this.GetComponent<HitBox>();
        hitBox.Health = health;
        //moveRigidBody= this.GetComponent<Rigidbody>();
        rigidBodys = this.GetComponentsInChildren<Rigidbody>();
        enemyAnimationManger = this.GetComponent<EnemyAnimationManger>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        foreach (var rigidBody in rigidBodys)
        {
           HitBox hitBox = rigidBody.gameObject.AddComponent<HitBox>();  
        }
        health.RagDoll = ragDoll;  
        leftHendColider.enabled = false;
        rightHendColider.enabled= false;
        isAttack = false;
        isCombat = false;
        leftHendColider.gameObject.AddComponent<EnemyWeapon>().Damage = damage;
        rightHendColider.gameObject.AddComponent<EnemyWeapon>().Damage = damage;
        materialColor = skinnedMeshRenderer.material.color;
    }

    protected virtual void SetIdle()
    {
        //BehaviorTree behaivortree = this.GetComponent<BehaviorTree>();
        //behaivortree.enabled = false;
    }
    protected virtual void ShowHitImpact()
    {
        blinkTimer -= Time.deltaTime;
        float _lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        float _intensity = (_lerp * blinkIntensity) + 1.0f;
        skinnedMeshRenderer.material.color = materialColor * Color.white * _intensity;
    }
    public virtual void Attack()
    {
        Debug.Log("공격");
    
        transform.LookAt(Player.Instance.transform);
        if (SkillManager.Instance.isHasShiled == false) //쉴드 적용중이 아니면 HP를 까시고 
        {
            Collider[] hitcoliders = Physics.OverlapSphere(this.transform.position, attackRadius);
            foreach (Collider collider in hitcoliders)
            {
                Debug.Log("콜라이더 검사 진행");
                if (collider.CompareTag("Player"))
                {
                    Debug.Log("플레이어 태그 검사");
                    PlayerHealth health = collider.GetComponent<PlayerHealth>()
                      ?? collider.GetComponentInParent<PlayerHealth>()
                         ?? collider.GetComponentInChildren<PlayerHealth>();  //??는 앞의 값이 null이면 다음 검사로 넘어가는 연산자
                    if (health != null)
                    {
                        if (health.GetComponent<Player>().IsDodge != false)
                        {
                            return;
                        }
                        health.TakeDamage(damage, Vector3.zero);
                        break;
                    }
                }

            }
        }
        else 
        {
            Collider[] hitcoliders = Physics.OverlapSphere(this.transform.position, attackRadius);
            foreach (Collider collider in hitcoliders)
            {
               
                if (collider.CompareTag("Shield"))
                {
                    Debug.Log("플레이어 태그 검사");
                    CollisonShieldInstantiate collisonShieldInstantiate = collider.GetComponent<CollisonShieldInstantiate>()
                      ?? collider.GetComponentInParent<CollisonShieldInstantiate>()
                         ?? collider.GetComponentInChildren<CollisonShieldInstantiate>();  //??는 앞의 값이 null이면 다음 검사로 넘어가는 연산자
                    if (collisonShieldInstantiate != null)
                    {
                        collisonShieldInstantiate.OnActiveCrackObject(GameManager.Instance.PlayerInputManager.transform);
                        UIManager.Instance.ShieldBar.TakeDamage(damage);                     
                        break;
                    }
                }

            }
        }

    }
    public void AttackAnimationStart()
    {
            enemyAnimationManger.Movement(false);
            enemyAnimationManger.Attack();        
    }
    public virtual void RoarAlert(float _roarRange)
    {
        Collider[] hitColiders = Physics.OverlapSphere(this.transform.position, _roarRange);
        foreach (Collider colider in hitColiders)
        {
            if (colider.gameObject.tag == "Enemy")
            {
                      
                if (colider.gameObject.GetComponent<Enemy>() == null)
                {
                    colider.gameObject.GetComponentInParent<Enemy>().isHeardRoar = true;
                }
                else
                {
                    colider.gameObject.GetComponent<Enemy>().isHeardRoar = true; 
                }
            }
                       
        }
        
    }
    protected virtual IEnumerator Recall()
    { 
        Transform spwanVFXtransform = this.transform; ;
        Vector3 startPosition = spwanVFXtransform.position;
        recallVFX = Instantiate(recallParticle, spwanVFXtransform.position,Quaternion.identity);
        Destroy(recallVFX, recallParticlesDestroyTime);
        yield return new WaitForSeconds(recallVFXTime); //리콜이 끝날때까지 기다리고

        spawnVFX = Instantiate(spawnPaticle, spwanVFXtransform.position, Quaternion.identity);
        Destroy(spawnVFX, spawnParticlesDestroyTime);
        StartCoroutine(SetSpawnMaterialValue());
        yield return new WaitForSeconds(spawnVFXTime);
       
    }
    #region Spawn을 위한 Material Setting함수
    /// <summary>
    /// 게임 오브젝트에 랜더러에 마테리얼을 스폰 효과가 적용된 마테리얼로 교체
    /// </summary>
    protected void ChangeSkinMeshMaterial(List<Material> _materials)
    {
        int count = 0;
        for (int i = 0; i < enemyInfo.skinnedMeshRenderers.Count; i++)
        {
            Material[] newMaterials = enemyInfo.skinnedMeshRenderers[i].materials; // 기존 배열 가져오기
            for (int j = 0; j < enemyInfo.skinnedMeshRenderers[i].materials.Length; j++)
            {               
                newMaterials[j] = _materials[count++];
                newMaterials[j].SetFloat("_SpiltValue", -0.8f);
            }
            enemyInfo.skinnedMeshRenderers[i].materials = newMaterials; // 새 배열을 다시 할당                    
        }
    }
    protected void ChangeWeaponMaterial(List<Material> _materials)
    {
        if (enemyInfo.weaponMeshRenderers != null)
        {
            int count = 0;
            for (int i = 0; i < enemyInfo.weaponMeshRenderers.Count; i++)
            {
                Material[] newMaterials = enemyInfo.weaponMeshRenderers[i].materials; // 기존 배열 가져오기
                for (int j = 0; j < enemyInfo.weaponMeshRenderers[i].materials.Length; j++)
                {
                    newMaterials[j] = _materials[count++];
                    newMaterials[j].SetFloat("_SpiltValue", -0.8f);
                }
                enemyInfo.weaponMeshRenderers[i].materials = newMaterials; // 새 배열을 다시 할당                    
            }
        }
    }
    protected virtual IEnumerator SetSpawnMaterialValue()
    {
        List<Material> materials = enemyInfo.spawnMaterial;
        yield return StartCoroutine(FadeMaterial(materials, startMaterialSpawnValue, endMaterialSpawnValue, spawnTime,enemyInfo.basicMaterial));     
    }
    protected virtual IEnumerator FadeMaterial(Material _material, float _startValue, float _endValue, float _spawnSpeed)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _spawnSpeed)
        {
            elapsedTime += _spawnSpeed * Time.deltaTime;
            float newValue = Mathf.Lerp(_startValue, _endValue, elapsedTime / _spawnSpeed);
            _material.SetFloat("_SpiltValue", newValue);
            yield return null;
        }
        _material.SetFloat("_SpiltValue", _endValue);
    }
    protected virtual IEnumerator FadeMaterial(List<Material> _material, float _startValue, float _endValue, float _spawnSpeed,List<Material> _basicMaterials)
    {
        
        float elapsedTime = 0f;
        while (elapsedTime < _spawnSpeed)
        {
            elapsedTime += _spawnSpeed * Time.deltaTime;
            foreach (var material in _material)
            {
                float newValue = Mathf.Lerp(_startValue, _endValue, elapsedTime / _spawnSpeed);
                material.SetFloat("_SpiltValue", newValue);
            }
            yield return null;
        }
        foreach (var material in _material)
        {
            material.SetFloat("_SpiltValue", _endValue);
        }
        ChangeSkinMeshMaterial(_basicMaterials);
        yield return SetWeaponMaterialValue();
    }
    #endregion
    protected virtual IEnumerator SetWeaponMaterialValue()
    {
        List<Material> materials = enemyInfo.weaponSpawnMaterial;

        yield return StartCoroutine(weaponFadeMaterial(materials, startMaterialSpawnValue, endMaterialSpawnValue, spawnTime, enemyInfo.weaponbasicMaterial));
    }
    protected virtual IEnumerator weaponFadeMaterial(List<Material> _material, float _startValue, float _endValue, float _spawnSpeed, List<Material> _basicMaterials)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _spawnSpeed)
        {
            elapsedTime += _spawnSpeed * Time.deltaTime;
            foreach (var material in _material)
            {
                float newValue = Mathf.Lerp(_startValue, _endValue, elapsedTime / _spawnSpeed);
                material.SetFloat("_SpiltValue", newValue);
            }
            yield return null;
        }
        foreach (var material in _material)
        {
            material.SetFloat("_SpiltValue", _endValue);
        }
        ChangeWeaponMaterial(_basicMaterials);
      
       
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // 디버그용 빨간색 원
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

}
