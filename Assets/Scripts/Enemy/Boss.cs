using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using MagicaCloth2;
using RootMotion.FinalIK;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static RootMotion.FinalIK.RagdollUtility;

namespace PS
{
    public class Boss : MonoBehaviour
    {
        [SerializeField]
        Health health;
        [SerializeField]
        HitBox hitBox;
        [SerializeField]
        private BossAnimationManger bossAnimationManger;
        [SerializeField]
        private BossVFXSpawnAbilities bossVFXSpawnAbilities;
        [SerializeField, Header("The distance between the player and me")]
        private float currentDistance;
        [SerializeField]
        private float maxDistance = 15f;
        private Vector3 targetDirection;
        private Vector3 destination;
        //public List<Skill> skillList;
        [SerializeField]
        private bool isGround;
        private Vector3 spherPos;
        private CapsuleCollider playerCapsuleColider;
        private TriAngleSkill triAngleSkill;
        [Header("땅과의 거리"), SerializeField]
        private float groundYoffset;
        public LayerMask groundMask;
        public Rigidbody rigidBody;
        public LookAtController lookAtController;
        public BossVFXSpawnAbilities BossVFXSpawnAbilities { get { return bossVFXSpawnAbilities; } }
        public BossAnimationManger BossAnimationManger { get { return bossAnimationManger; } }
        public TriAngleSkill TriAngleSkill { get { return triAngleSkill; } set { triAngleSkill = value; } }
        public float CurrentDistance { get { return currentDistance; } set { currentDistance = value; } }
        public float MaxDistance { get { return maxDistance; } set { maxDistance = value; } }
        public bool isCoolTime = false;
        public bool IsGround { get { return isGround; } set { isGround = value; } }
        [SerializeField]
        public EnemyInfo enemyInfo;
        protected GameObject recallVFX;
        [SerializeField]
        private void Awake()
        {
            Init();
        }
        private void OnEnable()
        {
            StartCoroutine(Recall());
        }
        private void Start()
        {
            SetCurrentDistance();
        }
        private void Update()
        {
            SetCurrentDistance(); //보스와 플레이어의 거리를 계산 
            CheckGround();

        }
        private void Init()
        {
            playerCapsuleColider = this.GetComponent<CapsuleCollider>();

            Player player = Player.Instance;
            if (player != null)
            {
                lookAtController.target = player.CameraFollowTarget.transform;
            }
            health = this.GetComponent<Health>();
            hitBox = this.GetComponent<HitBox>();
            hitBox.Health = health;
            triAngleSkill = this.GetComponent<TriAngleSkill>();
            triAngleSkill.enabled = false;
        }
        private void SetCurrentDistance()
        {
            Player player = Player.Instance;
            currentDistance = Vector3.Distance(this.transform.position, player.transform.position);
            targetDirection = (player.transform.position - this.transform.position).normalized;
        }
        public void RotateToDestination(GameObject obj, Vector3 destination, bool onlyY)
        {
            var direction = destination - obj.transform.position;
            var rotation = Quaternion.LookRotation(direction);

            if (onlyY)
            {
                rotation.x = 0;
                rotation.z = 0;
            }
            obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, rotation, 1);
        }
        #region 스킬 유틸리티 함수
        //스킬범위를 그린다.
        public void DrawTelegraph(int _selectNumber)
        {
            bossVFXSpawnAbilities.VFXSelecter(_selectNumber);// 해당 넘버의 스킬을 선택 

            RaycastHit hit;
            Ray ray = new Ray(transform.position, targetDirection); //나와 플레이어의 위치로 쏜다
            switch (_selectNumber)
            {
                case 1:
                case 2:
                case 3:
                    if (Physics.Raycast(ray, out hit, 10000))
                    {
                        destination = hit.point; //목표 방향 설정 
                        bossVFXSpawnAbilities.vfxMarker.transform.position = bossVFXSpawnAbilities.effectToSpawn.VFXMarkerPosition.position;
                        RotateToDestination(this.gameObject, destination, true);
                        RotateToDestination(bossVFXSpawnAbilities.vfxMarker, destination, true);
                    }
                    break;
                case 4:
                    if (Physics.Raycast(ray, out hit, 10000))
                    {
                        Player player = Player.Instance;
                        destination = hit.point; //목표 방향 설정
                        bossVFXSpawnAbilities.vfxMarker.transform.position = player.transform.position;
                        RotateToDestination(this.gameObject, destination, true);
                        RotateToDestination(bossVFXSpawnAbilities.vfxMarker, destination, true);
                    }
                    break;
            }
            bossVFXSpawnAbilities.vfxMarker.gameObject.SetActive(true);
        }
        public void EraseTelegraph()
        {
            bossVFXSpawnAbilities.vfxMarker.gameObject.SetActive(false);
        }
        public IEnumerator SkillDelay(float _dealyTime, Action _callback)
        {
            isCoolTime = true;
            yield return new WaitForSeconds(_dealyTime);
            isCoolTime = false;
            if (_callback != null)
            {
                _callback?.Invoke();
            }

        }
        #endregion
        #region 점프 관련 변수 및 함수들 
        private float startY; //처음 Y(높이) 저장
        private float timeElapsed = 0f; // 점프 시간 트래킹 
        private List<Vector3> trajectoryPoints = new List<Vector3>(); //포물선 궤적 저장용 
        public bool isJumping = false;
        [Header("점프 높이 sin 곡선 진폭")]
        public float jumpAmplitude = 3f;
        [Header("점프 속도 sin 곡선 주파수")]
        public float jumpFrequency = 2f;
        [Header("앞으로 이동하는 속도")]
        public float forwardSpeed = 3f;
        private Vector3 startPosition;
        private Vector3 endPosition;
        private Vector3 direction;


        private float jumpTime;
        public float jumpHeight;
        public void StartJump()
        {
            bossAnimationManger.Jump();
            isJumping = true;
             rigidBody.useGravity = false;// 리지드바디 안쓸거면 중력 끄기
            timeElapsed = 0f;    
            startY = transform.position.y;
            trajectoryPoints.Clear();
            startPosition = this.transform.position;
            endPosition = Player.Instance.transform.position;
            direction = (endPosition - startPosition).normalized;
        }
        //public void LerpJumpStart()
        //{
        //    startPosition = this.transform.position;
        //    endPosition = Player.Instance.transform.position;

        //    float distance = Vector3.Distance(new Vector3(startPosition.x, 0, startPosition.z), new Vector3(endPosition.x, 0, endPosition.z));
        //    jumpTime =  distance / forwardSpeed;
        //    isJumping = true;
        //    timeElapsed = 0f;

        //}
        public void MoveInSineWave(float _jumpFrequency ,float _jumpAmplitude ,float _forwardSpeed)
        {
       
            if (isJumping == true)
            {
                timeElapsed += Time.deltaTime;

                float newX = transform.position.x + (_forwardSpeed * direction.x * Time.deltaTime);
                float newY = startY + Mathf.Sin(timeElapsed * _jumpFrequency) * _jumpAmplitude; // y축 곡선형태로 오르낙 내리락 하게됨
                float newZ = transform.position.z + (_forwardSpeed * direction.z * Time.deltaTime);  // 지정한 속도로 전진   
                 transform.position = new Vector3(newX, newY, newZ) ; // 계산한거 위치 적용  
                if (timeElapsed > Math.PI / _jumpFrequency) //점프 끝 
                {
                    isJumping = false;
                    transform.position = new Vector3(newX, 0f, newZ);

                }
            }

        }
        //public void LerpJump()
        //{
        //    timeElapsed += Time.deltaTime;
        //    Vector3 horizontalPosition = Vector3.Lerp(startPosition ,endPosition,timeElapsed/jumpTime);
        //    float verticalPosition = jumpHeight * 4 * (timeElapsed / jumpTime) * (1 - timeElapsed / jumpTime);
        //    transform.position = new Vector3(horizontalPosition.x, startPosition.y + verticalPosition, horizontalPosition.z);

        //    if (timeElapsed >= jumpTime)
        //    {
        //        isJumping = false;
        //    }
        //    // 점프 완료 시 멈춤

        //}

        #endregion
        public bool CheckGround()
        {

            spherPos = new Vector3(transform.position.x, transform.position.y - groundYoffset, transform.position.z);
            if (Physics.CheckSphere(spherPos, playerCapsuleColider.radius - 0.05f, groundMask))
            {
                isGround = true;
                isJumping = false;
                return isGround;
            }
            else
            {
                isGround = false;
                isJumping = true;
                return false;
            }
        }
        void OnDrawGizmos()
        {
            if (trajectoryPoints.Count > 1)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < trajectoryPoints.Count - 1; i++)
                {
                    Gizmos.DrawLine(trajectoryPoints[i], trajectoryPoints[i + 1]);
                }
            }
        }
        #region 스폰 관련
        [SerializeField]
        protected GameObject recallParticle;
        [SerializeField]
        protected float recallParticlesDestroyTime = 2f;
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
        protected float startMaterialSpawnValue; //스폰 시작 값
        [SerializeField]
        protected float endMaterialSpawnValue;  // 엔드 값 
        [SerializeField]
        protected float spawnTime;
        public void SpawnInit()
        {

            if (enemyInfo.weaponMeshRenderers != null)
            {
                ChangeWeaponMaterial(enemyInfo.weaponSpawnMaterial);
            }
            SetIdle();
            ChangeSkinMeshMaterial(enemyInfo.spawnMaterial);
            StartCoroutine(Recall());

        }
        protected IEnumerator Recall()
        {

            Transform spwanVFXtransform = this.transform; 
            Vector3 startPosition = spwanVFXtransform.position;
            recallVFX = Instantiate(recallParticle, spwanVFXtransform.position, Quaternion.identity);
            Destroy(recallVFX, recallParticlesDestroyTime);
            yield return new WaitForSeconds(recallVFXTime); //리콜이 끝날때까지 기다리고

            spawnVFX = Instantiate(spawnPaticle, spwanVFXtransform.position, Quaternion.identity);
            Destroy(spawnVFX, spawnParticlesDestroyTime);
            //StartCoroutine(SetSpawnMaterialValue());
            yield return new WaitForSeconds(spawnVFXTime);
        }

        private void SetIdle()
        {
            BehaviorTree behaivortree = this.GetComponent<BehaviorTree>();
            behaivortree.enabled = false;

        }
        private void ChangeSkinMeshMaterial(List<Material> _materials)
        {
            int count = 0;
            for (int i = 0; i < enemyInfo.skinnedMeshRenderers.Count; i++)
            {
                Material[] newMaterials = enemyInfo.skinnedMeshRenderers[i].materials; // 기존 배열 가져오기
                for (int j = 0; j < enemyInfo.skinnedMeshRenderers[i].materials.Length; j++)
                {
                    //newMaterials[j] = enemyInfo.spawnMaterial[count].SetFloat("_SpiltValue", -0.8f);
                    //newMaterials[j] = enemyInfo.spawnMaterial[count++];
                    newMaterials[j] = _materials[count++];
                    newMaterials[j].SetFloat("_SpiltValue", -0.8f);
                }
                enemyInfo.skinnedMeshRenderers[i].materials = newMaterials; // 새 배열을 다시 할당                    
            }
        }
        private void ChangeWeaponMaterial(List<Material> _materials)
        {
            if (enemyInfo.weaponMeshRenderers != null)
            {
                int count = 0;
                for (int i = 0; i < enemyInfo.weaponMeshRenderers.Count; i++)
                {
                    Material[] newMaterials = enemyInfo.weaponMeshRenderers[i].materials; // 기존 배열 가져오기
                    for (int j = 0; j < enemyInfo.weaponMeshRenderers[i].materials.Length; j++)
                    {
                        //newMaterials[j] = enemyInfo.spawnMaterial[count].SetFloat("_SpiltValue", -0.8f);
                        //newMaterials[j] = enemyInfo.spawnMaterial[count++];
                        newMaterials[j] = _materials[count++];
                        newMaterials[j].SetFloat("_SpiltValue", -0.8f);
                    }
                    enemyInfo.weaponMeshRenderers[i].materials = newMaterials; // 새 배열을 다시 할당                    
                }
            }
        }
        private IEnumerator SetSpawnMaterialValue()
        {
            List<Material> materials = enemyInfo.spawnMaterial;
            yield return StartCoroutine(FadeMaterial(materials, startMaterialSpawnValue, endMaterialSpawnValue, spawnTime, enemyInfo.basicMaterial));
        }
        private IEnumerator FadeMaterial(Material _material, float _startValue, float _endValue, float _spawnSpeed)
        {
            float elapsedTime = 0f;
            while (elapsedTime < _spawnSpeed)
            {
                elapsedTime += _spawnSpeed * Time.deltaTime;
                //lerp 잘못알고있음  -> 
                float newValue = Mathf.Lerp(_startValue, _endValue, elapsedTime / _spawnSpeed);
                _material.SetFloat("_SpiltValue", newValue);
                yield return null;
            }
            _material.SetFloat("_SpiltValue", _endValue);
        }
        private IEnumerator FadeMaterial(List<Material> _material, float _startValue, float _endValue, float _spawnSpeed, List<Material> _basicMaterials)
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
        protected virtual IEnumerator SetWeaponMaterialValue()
        {
            List<Material> materials = enemyInfo.weaponSpawnMaterial;

            yield return StartCoroutine(weaponFadeMaterial(materials, startMaterialSpawnValue, endMaterialSpawnValue, spawnTime, enemyInfo.weaponbasicMaterial));
            #endregion
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
            BehaviorTree behaivortree = this.GetComponent<BehaviorTree>();
            behaivortree.enabled = true;

        }
    }
}
