using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using MagicaCloth2;
using PS;
using static PS.Player;

namespace PS
{
    public partial class Player : MonoBehaviour
    {

        [Header("플레이어 인풋 매니저"), SerializeField]
        private PlayerInputManager playerInputManager;
        [Header("플레이어 애니메이션 매니저")]
        private PlayerAnimationManager playerAnimationManager;
        [Header("플레이어 사격 매니저")]
        private PlayerShootManager playerShootManager;
        [Header("플레이어 Health")]
        private PlayerHealth playerHealth;
        //[Header("FinalIk 매니저")]
        //private IKManager ikManager;
        [Header("웨폰IK Manager")]
        private WeaponIk weaponIk;
        [Header("플레이어 리지드바디"), SerializeField]
        private Rigidbody plyaerRigidbody;
        [Header("플레이어 콜라이더"),SerializeField]
        private CapsuleCollider playerCapsuleColider;
        [Header("공중여부"), SerializeField]
        private bool isGround = false;
        [Header("달리기 여부"), SerializeField]
        private bool isDash = false;
        [Header("땅과의 거리"), SerializeField]
        private float groundYoffset;
        [Header("구르는 중"), SerializeField]
        private bool isDodge = false;
        private Vector3 dogeVector;
        [Header("사격 중"), SerializeField]
        private bool isFire = false;
        [Header("전투중"), SerializeField]
        private bool isCombat = false;
        private bool isCanBeHit;
        [Header("stemina"), SerializeField]
        private float maxStemina;
        [SerializeField]
        private float currentStemina;
        [Header("UseSkill")]
        private bool isUseSkill; 
        //private float dodgeTimer;
        // [SerializeField]
        //private AnimationCurve dodgeCurve;
        [Header("중력"), SerializeField]
        private float gravity = -9.81f;
        private Vector3 gravityVelocity;
        private Vector3 spherPos;
        [SerializeField, Header("카메라 컨트롤러")]
        private CameraController cameraController;
        [SerializeField, Header("이동방향"), Tooltip("모든 계산이 끝난 최종방향입니다.")]
        private Vector3 finalDirection;
        [SerializeField]
        private Transform cameraFollowTarget;
       

        public static Player Instance;
        public PlayerHealth PlayerHealth { get { return playerHealth; } set { playerHealth = value; } }
        public Rigidbody PlayerRigidbody { get { return plyaerRigidbody; } }
        public PlayerShootManager PlayerShootManager { get { return playerShootManager; } }
        public PlayerAnimationManager PlayerAnimationManager { get { return playerAnimationManager; } set { playerAnimationManager = value; } }
        public PlayerInputManager PlayerInputManager { get { return playerInputManager; } set { playerInputManager = value; } }
        public bool IsDodge { get { return isDodge; } set { isDodge = value; } }
        public bool IsGround { get { return isGround; } set { isGround = value; } }
        public bool IsDash { get { return isDash; } set { isDash = value; } }
        public bool IsFire { get { return isFire; } set { isFire = value; } }
        public bool IsUseSkill { get { return isUseSkill; } set { isUseSkill = value; } }
        public Vector3 FinalDirection { get { return finalDirection; } set { finalDirection = value; } }
        public CameraController CameraController { get { return cameraController; } set { cameraController = value; } }
        public Transform CameraFollowTarget { get { return cameraFollowTarget; } }
        public float MaxStemina { get { return maxStemina; } set { maxStemina = value; } }
        public float CurrentStemina { get { return currentStemina; } set { currentStemina = value; } }

        public PlayerSkill playerSkill;
        public LayerMask groundMask;
        public FullBodyBipedIK fullBodyBipedIK;

        public Vector3 DogeVector { get { return dogeVector; } }
        [SerializeField, Header("이동 속도")]
        private float speed = 10f;
        [SerializeField, Header("회피 속도")]
        private float dodgeSpeed = 5f;
        [SerializeField, Header("회전 속도")]
        private float rotSpeed = 3f;
        [SerializeField, Header("대쉬 속도")]
        private float dashSpeed;
        [SerializeField, Header("대쉬 지속 시간")]
        private float dashDurationTime;
        private float turnSmoothVelocity;

        private Transform playerTransform;
        public Transform PlayerTransform { get { return playerTransform; } }
        public enum PlayerState
        {
            IDLE,
            COMBAT,
            DIE
        }
        public PlayerState playerState;

        private void Awake()
        {
            Init();
        }
        private void Update()
        {
            if (currentStemina < 0)
            {
                DashOut();
            }
            UpdatePlayerState();
        }
        private void FixedUpdate() //물리 업데이트가 필요할때 많이 쓰는듯? 
        {
            UpdatePhysicsState();
        }
        public void Init()
        {
            isUseSkill = false; 
            isCanBeHit = true;
            Instance = this;
            //playerInputManager = this.GetComponent<PlayerInputManager>();
            playerAnimationManager = this.GetComponent<PlayerAnimationManager>();
            playerShootManager = this.GetComponent<PlayerShootManager>();
            //plyaerRigidbody = this.GetComponent<Rigidbody>();
            //playerCapsuleColider = this.GetComponent<CapsuleCollider>();
            //ikManager = this.GetComponent<IKManager>();
            weaponIk = this.GetComponent<WeaponIk>();
            playerHealth = this.GetComponent<PlayerHealth>();
            SetPlayerState(PlayerState.IDLE);
            //cameraController = Camera.main.GetComponent<CameraController>();
            //aimTarget = null;
            //KeyFrame? -> 애니메이션 또는 변화를 표현하기 위해 시간 축상 특정값을 정의하는 점을 나타냅니다.주로 애니메이션 커브와 함께 사용
            //특정 애니메이션에  애니메이션 커브를 가져와 해당 애니메이션 시간을  구르기 시간으로 설정할려는 것
            //보류
            //Keyframe dodgeLastFrame = dodgeCurve[dodgeCurve.length - 1];
            //dodgeTimer = dodgeLastFrame.time;
            //if (aimTarget == null)
            //{
            //    GameObject newAimTarget = new GameObject("AimTarget");
            //    aimTarget = newAimTarget.transform;
            //}
            currentStemina = maxStemina;
        }

        //private void PlayerRotation()
        //{
        //    if (playerInputManager.Dir != Vector3.zero) // 입력이 있을경우 
        //    {
        //        // 바로 뒤돌경우 회전이 즉각적으로 이루어지지 않음 그에따른 처리  -> 역방향 회전은 미리 돌려놓는다.
        //        //mathf는 양수 음수에 따라 1,0,-1 
        //        if (Mathf.Sign(transform.forward.x) != Mathf.Sign(playerInputManager.Dir.x) || Mathf.Sign(transform.forward.z) != Mathf.Sign(playerInputManager.Dir.z))
        //        {
        //           transform.Rotate(0, 1, 0);
        //        }
        //        //transform.forward = dir;  이 오브젝트에 앞방형은  dir 즉 입력한 방향이다. 
        //        //Vector3.Lerp 회전 보간이 들어가고 
        //        transform.forward = Vector3.Slerp(transform.forward, playerInputManager.Dir, Time.deltaTime * rotSpeed);

        //    }
        //}
        private void PlayerMovement()
        {
             Vector3 desiredDirection;
             desiredDirection = cameraController.YRotation * playerInputManager.Dir;
             plyaerRigidbody.MovePosition(this.gameObject.transform.position + desiredDirection * speed * Time.fixedDeltaTime);
        }
        private void PlayerRotation()
        {

            if (playerInputManager.Dir != Vector3.zero && playerState == PlayerState.IDLE)
            {
                Vector3 desiredDirection;
                if (playerState == PlayerState.IDLE)
                {
                    //Vector3 playerVelocity = new Vector3(playerInputManager.Dir.x,0f,playerInputManager.Dir.z);
                    desiredDirection = cameraController.YRotation * playerInputManager.Dir;
                    //transform.rotation = Quaternion.LookRotation(playerVelocity);
                    transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(desiredDirection), rotSpeed * Time.deltaTime);
                }

            }
            else if (playerState == PlayerState.COMBAT)
            {
                // desiredDirection = cameraController.YRotation * playerInputManager.Dir;
                // transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(desiredDirection), rotSpeed * Time.deltaTime);
                //var targetRotation = cameraController.transform.eulerAngles.y;

                //transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation,
                //ref turnSmoothVelocity, rotSpeed *Time.deltaTime);

                //Vector3 playerVelocity = new Vector3(playerInputManager.Dir.x,0f,playerInputManager.Dir.z);
                //캐릭터의 회전에 따른 바라보는 방향을 얻기"로 번역할 수 있습니다. 
                //desiredDirection = cameraController.YRotation * playerInputManager.Dir;
                //바라보는 방향과 캐릭터의 전방 벡터 사이의 각도를 구하기"
                //float angle = Mathf.Atan2(desiredDirection.x, desiredDirection.z) * Mathf.Rad2Deg;
                //Vector3 direction = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));

                //transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(desiredDirection), rotSpeed * Time.deltaTime);

                transform.rotation = Quaternion.Slerp(this.transform.rotation, cameraController.YRotation, rotSpeed * Time.fixedDeltaTime);

            }
        }
        private bool CheckGround()
        {
            spherPos = new Vector3(transform.position.x, transform.position.y - groundYoffset, transform.position.z);

            if (Physics.CheckSphere(spherPos, playerCapsuleColider.radius - 0.05f, groundMask))
            {
                isGround = true;
                return isGround;
            }
            else
            {
                isGround = false;           
                return false;
            }
        }
        //private void CheckGround()
        //{
        //    RaycastHit hit;

        //    if (Physics.Raycast(transform.position + (Vector3.up * 0.2f), Vector3.down, out hit, 0.4f, groundMask))
        //    {
        //        isGround = true;
        //    }
        //    else 
        //    {
        //        isGround = false;
        //    }

        //}   
        private void Gravity()
        {
            if (CheckGround() == false )
            {
                gravityVelocity.y += gravity * Time.deltaTime;  //공중이면 y에 가속도를 붙혀주겠다 

            }
            else if (gravityVelocity.y < 0)
            {
                gravityVelocity.y = -2f;
            }
         
            plyaerRigidbody.MovePosition(gravityVelocity * Time.deltaTime); //실시간 중력 적용

        }
        #region Doge 관련 변수 및  함수 
        public void DodgeIn()
        {
            if (CheckGround() == true && playerInputManager.Dir != Vector3.zero && isDodge == false)
            {
                SetPlayerState(PlayerState.IDLE);
                isDodge = true;
                //this.transform.GetComponent<CapsuleCollider>().enabled = false; ->  
                dogeVector = playerInputManager.Dir;
                playerAnimationManager.Dodge();
                float dogeTime = playerAnimationManager.PlayerAnimator.GetCurrentAnimatorClipInfo(0).Length;
                Vector3 dodgePower = dogeVector * dodgeSpeed;
                plyaerRigidbody.AddForce(dodgePower, ForceMode.Impulse);
                Invoke(nameof(DodgeOut), 1f);
            }
        }
        private void DodgeOut()
        {
            //this.transform.GetComponent<CapsuleCollider>().enabled = true;
            isDodge = false;
        }
        #endregion
        public void SetPlayerState(PlayerState _playerState)
        {
            playerState = _playerState;
        }
        private void UpdatePhysicsState()
        {
            playerAnimationManager.InAir(isGround);
            Gravity();
            switch (playerState)
            {
                case PlayerState.IDLE:
                    ///playerShootManager.AimIK.solver.IKPositionWeight = 0f;
                    //playerShootManager.AimIK.solver.Update();
                    //ikManager.AimIK.solver.IKPositionWeight = 0f;
                    //ikManager.headLookWeight = 0f;
                    weaponIk.iterations = 0;
                    PlayerRotation();
                    PlayerMovement();
                    break;
                case PlayerState.COMBAT:
                    //ikManager.AimIK.solver.IKPositionWeight = 1f;
                    //ikManager.headLookWeight = 1f;
                    //playerShootManager.AimIK.solver.target = playerShootManager.AimTransform;
                    //playerShootManager.AimPose();
                    //playerShootManager.AimIK.solver.Update();
                    weaponIk.iterations = 10;
                    PlayerRotation();
                    PlayerMovement();
                    break;
                case PlayerState.DIE:
                    break;
            }

        }
        private void UpdatePlayerState()
        {
            switch (playerState)
            {
                case PlayerState.IDLE:
                    isCombat = false;
                    playerAnimationManager.Combat(false);
                    playerAnimationManager.PlayerAnimator.SetLayerWeight(1, 1f);
                    //Mathf.Clamp01(value)  -> 주어진 값을 0 ~ 1로 제한
                    playerAnimationManager.SetMovementAnimatorValue(Mathf.Clamp01(Mathf.Abs(playerInputManager.Dir.x)), Mathf.Clamp01(Mathf.Abs(playerInputManager.Dir.z)));
                    break;
                case PlayerState.COMBAT:
                    if (SkillManager.Instance.IsHealing == true) //힐스킬 시전중
                    {
                        isCombat = false;
                        playerAnimationManager.Combat(false);
                    }
                    else
                    {
                        isCombat = true;
                        playerAnimationManager.Combat(true);
                    }
                    playerAnimationManager.PlayerAnimator.SetLayerWeight(1, 1f);
                    playerAnimationManager.SetMovementAnimatorValue(playerInputManager.Dir.x, playerInputManager.Dir.z);
                    break;
                case PlayerState.DIE:
                    break;
            }
        }
        //public IEnumerator Dodge()
        //{
        //    isDodge = true;
        //    playerAnimationManager.Dodge();
        //    float dogeTime = playerAnimationManager.PlayerAnimator.GetCurrentAnimatorClipInfo(0).Length;
        //    Vector3 dashPower = this.transform.forward * dodgeSpeed;
        //    plyaerRigidbody.AddForce(dashPower, ForceMode.VelocityChange);
        //    Debug.Log(dogeTime);
        //    yield return new WaitForSeconds(dogeTime);
        //    playerAnimationManager.PlayerAnimator.ResetTrigger("Dodge");
        //    isDodge = false;
        //}
        #region 대쉬 관련 변수 및 함수
        private Vector3 delayedForceToApply;
        public void DashIn()
        {
            SetPlayerState(PlayerState.IDLE);
            DealyDash();
            //Vector3 desiredDirection = cameraController.YRotation * playerInputManager.Dir;

            //delayedForceToApply = desiredDirection;

            //Invoke(nameof(DealyDash), 0.025f); // 파악하고 튀어나가는 느낌을 극대화 할려고 딜레이줌

            //Invoke(nameof(DashOut), dashDurationTime); //LeftShift를 땠을경우로 조건이 바뀜 

        }
        private void DealyDash()
        {
            if (isDash == true)
            {
                return;
            }
            else
            {
                isDash = true;
                playerAnimationManager.Dash(isDash);
                //plyaerRigidbody.AddForce(delayedForceToApply * dashSpeed, ForceMode.Impulse);
                speed *= 2f;
            }

        }
        public void DashOut()
        {
            speed /= 2f;
            isDash = false;
            playerAnimationManager.Dash(isDash);
        }
        #endregion

        private void OnTriggerEnter(Collider other)
        {
            bool isCheck = playerState != PlayerState.DIE && other.tag == "Enemy" && SkillManager.Instance.IsShield == false;

            if (isCheck)
            {
                EnemyWeapon enemyWeapon = other.gameObject.GetComponent<EnemyWeapon>();
                float damage = enemyWeapon.Damage;
                playerHealth.TakeDamage(damage, other.gameObject.transform.forward);
                Debug.Log("맞았음");
            }
            

        }

        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(spherPos, playerCapsuleColider.radius - 0.05f);
        //}
        
       
    }
}
