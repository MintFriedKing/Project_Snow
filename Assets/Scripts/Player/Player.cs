using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using MagicaCloth2;
using PS;
using static PS.Player;
using Unity.VisualScripting;

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
        [Header("플레이어 콜라이더"), SerializeField]
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

        private void OnValidate()
        {
#if UNITY_EDITOR
            if(animHandler == null )
            {
                animHandler = GetComponent<AnimHandler>();
            }
#endif
        }
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

            if (animHandler == null)
            {
                animHandler = GetComponent<AnimHandler>();
            }

            animHandler.ReloadEvent = () =>
            {
                ChangeReload(true);
            };

            isUseSkill = false;
            isCanBeHit = true;
            Instance = this;
            playerAnimationManager = this.GetComponent<PlayerAnimationManager>();
            playerShootManager = this.GetComponent<PlayerShootManager>();     
            weaponIk = this.GetComponent<WeaponIk>();
            playerHealth = this.GetComponent<PlayerHealth>();
            SetPlayerState(PlayerState.IDLE);
            currentStemina = maxStemina;
        }
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
                    desiredDirection = cameraController.YRotation * playerInputManager.Dir;   
                    transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(desiredDirection), rotSpeed * Time.deltaTime);
                }

            }
            else if (playerState == PlayerState.COMBAT)
            {            
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

        private void Gravity()
        {
            if (CheckGround() == false)
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
                dogeVector =cameraController.YRotation * playerInputManager.Dir;
                playerAnimationManager.Dodge();
                float dogeTime = playerAnimationManager.PlayerAnimator.GetCurrentAnimatorClipInfo(0).Length;
                Vector3 dodgePower = dogeVector * dodgeSpeed;
                plyaerRigidbody.AddForce(dodgePower, ForceMode.Impulse);
                Invoke(nameof(DodgeOut), dogeTime);
            }
        }
        private void DodgeOut()
        {
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
                    weaponIk.iterations = 0;
                    PlayerRotation();
                    PlayerMovement();
                    break;
                case PlayerState.COMBAT:
                    weaponIk.iterations = 10;
                    PlayerRotation();
                    PlayerMovement();
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
                //case PlayerState.DIE:
                //    break;
            }
        }
    
        #region 대쉬 관련 변수 및 함수
        private Vector3 delayedForceToApply;
        public void DashIn()
        {
            SetPlayerState(PlayerState.IDLE);
            DealyDash();
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
        AnimHandler animHandler;

        public bool isReload = false;

        public void ChangeReload(bool _reload)
        {
            this.isReload = _reload;
        }

        public bool IsReloading()
        {
            return isReload;
        }

    }
}
