using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.AI;
using RootMotion.FinalIK;
using Unity.VisualScripting;
using System;
using BehaviorDesigner.Runtime;

namespace PS
{
  
    public class RangeEnemy : Enemy
    {
        private RangeEnemyAnimationManager rangeEnemyAnimationManager;
        [Header("Target Transform")]
        public Transform aimTarget;
        public RangeEnemyAnimationManager RangeEnemyAnimationManager { get { return rangeEnemyAnimationManager; } set { rangeEnemyAnimationManager = value; } }

        #region 조준 관련 변수
        [Header("AimIK"), SerializeField]
        private AimIK aimIK;
        [SerializeField, Header("[AimPoser]"), Tooltip("AimPoser는 방향에 따라 애니메이션 이름을 반환하는 도구라고 한다.")]
        private AimPoser aimPoser;
        private AimPoser.Pose aimPose;
        private AimPoser.Pose lastPose;
        [SerializeField, Header("Look At IK")]
        private LookAtIK lookAtIK;
        private FullBodyBipedIK fullBodyBipedIK;
        public FullBodyBipedIK FullBodyBipedIK { get { return fullBodyBipedIK; } set { fullBodyBipedIK = value; } }
        public AimIK AimIK { get { return aimIK; } set { aimIK = value; } }
        public LookAtIK LookAtIK { get { return lookAtIK; } set { lookAtIK = value; } }
        public AimPoser AimPoser { get { return aimPoser; } set { aimPoser = value; } }
        public AimPoser.Pose _AimPose { get { return aimPose; } set { aimPose = value; } }
        [Header("[Will keep the aim target at a distance]"), Tooltip("너무 가까우면 조준자세가 무너질수 있어 최소한의 거리를 만듬")]
        public float minAimDistance = 0.5f;
        [Header("Time of cross-fading from pose to pose."), Tooltip("포즈 간 크로스 페이딩의 지속 시간.??")]
        public float crossfadeTime = 0.2f;
        #endregion
        #region 사격관련 멤버 변수
        [Header("Fire Transform"), SerializeField]
        private Transform fireTransform;
        [Header("Bullet Prefab"), SerializeField]
        private GameObject bulletPrefab;
        [SerializeField, Header("muzzle flash")]
        private ParticleSystem[] muzlePaticleSystems;
        [SerializeField, Header("Hit Effect")]
        private ParticleSystem hitEffectPacticleSystem;
        private LayerMask excludeTarget;
        public Transform FireTransform { get { return fireTransform; } set { fireTransform = value; } }
        public GameObject BulletPrefab { get { return bulletPrefab; } set { bulletPrefab = value; } }
        public float fireRate = 1.5f;
        public float nextFireCheckTime;
        public float aimError = 2f;
        public float bulletSpeed = 20f;
        #endregion
        private void OnEnable()
        {
            SpawnInit();
        }
        private void Awake()
        {
            Init();
        }
        private void Start()
        {
            rangeEnemyAnimationManager.Movement(0f);
        }
        private void Update()
        {
            if (aimTarget == null)
            {
                return;
            }
            if (Time.time >= nextFireCheckTime)
            {
                //1을 RPS로 나눠 발사 간격 계산
                nextFireCheckTime = Time.time + (1f / fireRate);
                isAttack = true;
            }
            //if (Time.time >= nextFireCheckTime)
            //{
            //    nextFireCheckTime = Time.time + fireRate;
            //    isAttack = true;
            //}
        }
        private void LateUpdate()
        {
            AimPose();
            aimIK.solver.Update();
            if (lookAtIK != null) lookAtIK.solver.Update();
        }
        protected override void Init()
        {
            health = this.GetComponent<Health>();
            ragDoll = this.GetComponent<RagDoll>();
            hitBox = this.GetComponent<HitBox>();
            hitBox.Health = health;
            rigidBodys = this.GetComponentsInChildren<Rigidbody>();
            rangeEnemyAnimationManager = this.GetComponent<RangeEnemyAnimationManager>();
            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            navMeshAgent = this.GetComponent<NavMeshAgent>();
            foreach (var rigidBody in rigidBodys)
            {
                HitBox hitBox = rigidBody.gameObject.AddComponent<HitBox>();
            }
            health.RagDoll = ragDoll;
            isAttack = false;
            isCombat = false;
            materialColor = skinnedMeshRenderer.material.color;
            aimTarget = null;
            aimIK = this.transform.GetComponent<AimIK>();
            aimPoser = this.GetComponent<AimPoser>();
            lookAtIK = this.GetComponent<LookAtIK>();
            aimIK.enabled = false;
            //lookAtIK.enabled = false;
            fullBodyBipedIK = this.GetComponent<FullBodyBipedIK>();
            //fullBodyBipedIK.enabled =  false;
            fullBodyBipedIK.solver.leftHandEffector.positionWeight = 0f;
         
        }
        public void SpawnInit()
        {
            //SetIdle();
            //ChangeSkinMeshMaterial(enemyInfo.spawnMaterial);
            //StartCoroutine(Recall());     
            //StartCoroutine(weaponFadeMaterial(enemyInfo.weaponSpawnMaterial, startMaterialSpawnValue,endMaterialSpawnValue,spawnTime,enemyInfo.weaponbasicMaterial));        

            if (enemyInfo.weaponMeshRenderers != null)
            {
                ChangeWeaponMaterial(enemyInfo.weaponSpawnMaterial);
            }
            SetIdle();
            ChangeSkinMeshMaterial(enemyInfo.spawnMaterial);
            StartCoroutine(Recall());

        }
        protected override void SetIdle()
        {
            //BehaviorTree behaivortree = this.GetComponent<BehaviorTree>();
            //behaivortree.enabled = false;
            aimIK.solver.target = null;
            aimIK.solver.IKPositionWeight = 0;
            aimIK.solver.Update();
            lookAtIK.solver.target = null;
            lookAtIK.solver.Update();
            fullBodyBipedIK.solver.leftHandEffector.positionWeight = 0;
            rangeEnemyAnimationManager.Movement(0f);
            rangeEnemyAnimationManager.Animator.SetLayerWeight(1, 0f);
        }
        protected override void ShowHitImpact()
        {
            base.ShowHitImpact();
        }
        public override void Attack() //원거리 공격이라 재정의 해야함 
        {
        
            if (isAttack == true)
            {
                aimTarget = aimIK.solver.target;
                //방향
                Vector3 aimDirection = (aimTarget.position - fireTransform.position).normalized;
                //가우시안분포없이 간단하게 명중률 조정 테스트
                aimDirection.x += UnityEngine.Random.Range(-aimError, aimError) * 0.01f;
                aimDirection.y += UnityEngine.Random.Range(-aimError, aimError) * 0.01f;
                //연출 
                foreach (var paritcle in muzlePaticleSystems)
                {
                    paritcle.Emit(1);

                }
                isAttack = false;
                //총알 생성 및 발사 
                GameObject bullet = Instantiate(bulletPrefab, fireTransform.position, Quaternion.identity);
                bullet.transform.gameObject.GetComponent<Bullet>().damage = Damage;
                bullet.GetComponent<Rigidbody>().velocity = aimDirection * bulletSpeed;
                Debug.Log("총 쏨");
            }


        }
        public override void RoarAlert(float _roarRange)
        {
            base.RoarAlert(_roarRange);
        }
        protected override IEnumerator SetSpawnMaterialValue()
        {
            return base.SetSpawnMaterialValue();
        }
        protected override IEnumerator FadeMaterial(List<Material> _material, float _startValue, float _endValue, float _spawnSpeed ,List<Material> _basicMaterial)
        {
            return base.FadeMaterial(_material, _startValue, _endValue, _spawnSpeed,_basicMaterial);
        }
        protected override IEnumerator FadeMaterial(Material _material, float _startValue, float _endValue, float _spawnSpeed)
        {
            return base.FadeMaterial(_material, _startValue, _endValue, _spawnSpeed);
        }
        protected override IEnumerator weaponFadeMaterial(List<Material> _material, float _startValue, float _endValue, float _spawnSpeed, List<Material> _basicMaterials)
        {
            return base.weaponFadeMaterial(_material, _startValue, _endValue, _spawnSpeed, _basicMaterials);
        }
        #region Final IK  -AimIK 관련 함수 - 현재 테스트중
        //조준 대상이 너무 가까이 있지 않도록 해야 합니다 (대상이 첫 번째 뼈보다 더 가깝고 마지막 뼈보다 더 멀 경우, 솔버가 불안정해질 수 있습니다). by Document
        private void LimitAimTarget()
        {
            Vector3 aImFrom = aimIK.solver.bones[0].transform.position;
            Vector3 direction = (aimIK.solver.IKPosition - aImFrom);
            direction = direction.normalized * Mathf.Max(direction.magnitude, minAimDistance);
            aimIK.solver.IKPosition = aImFrom + direction;
        }
        public void DirectCrossFade(string _state, float _target)
        {
            float f = Mathf.MoveTowards(RangeEnemyAnimationManager.Animator.GetFloat(_state), _target,
                Time.deltaTime * (1f / crossfadeTime));

            RangeEnemyAnimationManager.Animator.SetFloat(_state, f);
        }

        public void AimPose()
        {
            LimitAimTarget();

            Vector3 direction = (aimIK.solver.IKPosition - aimIK.solver.bones[0].transform.position);

            Vector3 localDirecion = transform.InverseTransformDirection(direction);

            aimPose = aimPoser.GetPose(localDirecion);

            // 에임애니메이션 변경방법? 
            if (aimPose != lastPose)
            {
                //포즈의 각도 버퍼를 늘려서 방향이 약간 바뀌더라도 너무 빨리 다시 전환되지 않도록 합니다.
                aimPoser.SetPoseActive(aimPose);
                // 포즈를 저장하여 변경 여부를 알 수 있도록 합니다.
                lastPose = aimPose;
            }

            // //?? 직접 블랜딩
            foreach (AimPoser.Pose pose in aimPoser.poses)
            {
                if (pose == aimPose)
                {
                    DirectCrossFade(pose.name, 1f);
                }
                else
                {
                    DirectCrossFade(pose.name, 0f);
                }
            }

            if (lookAtIK != null)
            {
                lookAtIK.solver.Update();
            }

        }
        #endregion
    } 
}
