using PS;
using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PS.Player;

public class PlayerShootManager : MonoBehaviour
{
    [Header("사격에서 제외할 레이어 마스크"), SerializeField]
    private LayerMask excludeTargetLayer;
    private Player player;
    private PlayerInputManager playerInputManager;
    private Camera playerCamera;
    [Header("에임포지션"), SerializeField]
    private Vector3 aimPosition;
    //[Header("AimIK"), SerializeField]
    //private AimIK aimIK;
    //[SerializeField, Header("[AimPoser]"), Tooltip("AimPoser는 방향에 따라 애니메이션 이름을 반환하는 도구라고 한다.")]
    //private AimPoser aimPoser;
    //private AimPoser.Pose aimPose;
    //private AimPoser.Pose lastPose;
    //[SerializeField, Header("Look At IK")]
    //private LookAtIK lookAtIK;

    public Vector3 AimPosition { get { return aimPosition; } }
    public Transform aimTransform;
    //public AimIK AimIK { get { return aimIK; } set { aimIK = value; } }
    [Header("[Will keep the aim target at a distance]"), Tooltip("너무 가까우면 조준자세가 무너질수 있어 최소한의 거리를 만듬")]
    public float minAimDistance = 0.5f;
    [Header("Time of cross-fading from pose to pose."), Tooltip("포즈 간 크로스 페이딩의 지속 시간.??")]
    public float crossfadeTime = 0.2f;

    public enum AimState
    {
        Idle,
        HipFire,
        ADS
    }
    public AimState aimState;
    public Gun gun;
    public bool isADS;
    private void Awake()
    {
        Init();
    }
    private void Update()
    {
        UpdateAim();
    }
    private void Init()
    {
        player = this.transform.GetComponent<Player>();
        playerInputManager = this.transform.GetComponent<PlayerInputManager>();
        playerCamera = Camera.main;
        aimState = AimState.Idle;
        isADS = false;
        //aimIK = this.transform.GetComponent<AimIK>();
        //aimPoser = this.GetComponent<AimPoser>();
        //lookAtIK = this.GetComponent<LookAtIK>();
        //aimIK.enabled = false;
        //lookAtIK.enabled = false;

    }
    public void UpdateAim()
    {
        // ViewportPointToRay 카메라상 위치를 정규화해서 값을 얻음 0.5 0.5 면 중앙
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
        if (Physics.Raycast(ray, out hit, gun.Range, ~excludeTargetLayer))
        {
            aimPosition = hit.point;
            aimTransform = hit.transform;
            //if (Physics.Linecast(gun.fireTramsform.position, hit.point, out hit, ~excludeTargetLayer))
            //{
            //    aimTransform = hit.transform;
            //    aimPosition = hit.point;
            //    //Debug.Log(hit.transform.name);
            //}
        }
        else //사정거리 안에 아무것도 없으면
        {
            aimPosition = playerCamera.transform.position + playerCamera.transform.forward * gun.Range;
            aimTransform = hit.transform;
        }
        //    //AimIK는Solver는Aim Ik 에셋이 목표를 향하도록 계산하는 엔진!! 
        //    aimIK.solver.IKPosition = aimTarget.position;
        //    aimIK.solver.IKPositionWeight = 1.0f;
    }

    public void Shoot() //플레이어 사격 
    {
        if (aimState == AimState.Idle) //기본 상태(사격 입력이 아직 없는경우)
        {
            aimState = AimState.HipFire;  // 사격중으로 상태 전환

        }
        else if (aimState == AimState.HipFire) //조준없이 사격
        {

            if (gun.Fire(aimPosition) == true) // aimPosition 위치를 기준으로 총 클래스가 사격에 관한 처리를 한다.(총구화염, 히트 이펙트 등)
            {

                Debug.Log("히트");

            }
            else
            {
                return;
            }
        }
    }

    #region Final IK  -AimIK 관련 함수 - 현재 테스트중이라 주석처리
    //조준 대상이 너무 가까이 있지 않도록 해야 합니다 (대상이 첫 번째 뼈보다 더 가깝고 마지막 뼈보다 더 멀 경우, 솔버가 불안정해질 수 있습니다). by Document
    //    private void LimitAimTarget()
    //    {
    //        Vector3 aImFrom = aimIK.solver.bones[0].transform.position;
    //        Vector3 direction = (aimIK.solver.IKPosition -aImFrom);
    //        direction = direction.normalized * Mathf.Max(direction.magnitude, minAimDistance);
    //        aimIK.solver.IKPosition = aImFrom + direction; 
    //    }
    //    private void DirectCrossFade(string _state,float _target)
    //    {
    //        float f = Mathf.MoveTowards(player.PlayerAnimationManager.PlayerAnimator.GetFloat(_state) ,_target,
    //            Time.deltaTime * (1f / crossfadeTime));

    //        player.PlayerAnimationManager.PlayerAnimator.SetFloat(_state, f);
    //    }

    //    public void AimPose()
    //    {
    //        LimitAimTarget();


    //        Vector3 direction = (aimIK.solver.IKPosition - aimIK.solver.bones[0].transform.position);

    //        Vector3 localDirecion = transform.InverseTransformDirection(direction);

    //        aimPose = aimPoser.GetPose(localDirecion);

    //       // 에임애니메이션 변경방법? 
    //        if (aimPose != lastPose)
    //        {
    //            //포즈의 각도 버퍼를 늘려서 방향이 약간 바뀌더라도 너무 빨리 다시 전환되지 않도록 합니다.
    //           aimPoser.SetPoseActive(aimPose);
    //           // 포즈를 저장하여 변경 여부를 알 수 있도록 합니다.
    //           lastPose = aimPose;
    //        }

    //        //?? 직접 블랜딩
    //       foreach (AimPoser.Pose pose in aimPoser.poses)
    //       {
    //           if (pose == aimPose)
    //            {
    //               DirectCrossFade(pose.name, 1f);
    //           }
    //            else
    //            {
    //                DirectCrossFade(pose.name, 0f);
    //           }
    //        }

    //       if (lookAtIK != null)
    //        {
    //            lookAtIK.solver.Update();
    //        }

    //}
    #endregion

  
}

