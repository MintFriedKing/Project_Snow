using PS;
using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKManager : MonoBehaviour
{
    // 3인칭 시점을 위한 기본 애니메이터 컨트롤러를 확장하여 IK를 추가합니다.
    [Range(0f, 1f), Tooltip("처다보는거 강도")]
    public float headLookWeight = 1f;
    public Vector3 gunHolderOffset;
    public Vector3 leftHandOffset;
    public Recoil recoil;

    private AimIK aimIk;
    private FullBodyBipedIK fullBodyBipedIK;
    private PlayerAnimationManager playerAnimationManager;
    private Player player;
    private Vector3 aimTarget; // 타겟 위치
    private Vector3 headLookAxis; //플레이어 시선 위치 
    private Vector3 leftHandPosRelToRightHand; //왼손 위치
    private Quaternion leftHandRotRelToRightHand; //왼손 로테이션 
    private Quaternion rightHandRotation;//오른손 로테이션

    public AimIK AimIK { get { return aimIk; } set { aimIk =value;} }
    public Vector3 AimTarget { get {  return aimTarget; } set { aimTarget = value;} }

    private void OnDestroy()
    {
        if (fullBodyBipedIK != null) fullBodyBipedIK.solver.OnPreRead -= OnPreRead;
    }
    private void Awake()
    {
        Init();
    }
 
    private void LateUpdate()
    {
        UpdateIK();
    }
    private void Init()
    { 
        // 컴포넌트 셋팅 
        aimIk =this.GetComponent<AimIK>();
        fullBodyBipedIK = this.GetComponent<FullBodyBipedIK>();
        fullBodyBipedIK.solver.OnPreRead += OnPreRead;
        playerAnimationManager = this.GetComponent <PlayerAnimationManager>();
        player = this.GetComponent<Player>();
        // IK 컴포넌트의 업데이트를 직접 관리하기 위해 비활성화합니다.
        aimIk.enabled = false;
        fullBodyBipedIK.enabled = false;

        // 시작 시 머리가 캐릭터의 전방을 향해 회전되어 있다고 가정합니다. -> 게임 시작시 머리 방향은 정면인듯? 
        headLookAxis = fullBodyBipedIK.references.head.InverseTransformVector(fullBodyBipedIK.references.root.forward);

    }
    // FBBIK가 해결하기 전에 최종 계산을 수행합니다. 
    // 반동(Recoil)은 이미 해결되었으므로 계산된 오프셋을 사용할 수 있습니다.  
    // 여기서는 왼손의 위치를 오른손의 위치와 회전에 상대적으로 설정합니다.
    private void OnPreRead()
    {
        Quaternion r = recoil != null ? recoil.rotationOffset * rightHandRotation : rightHandRotation;
        Vector3 leftHandTarget = fullBodyBipedIK.references.rightHand.position + fullBodyBipedIK.solver.rightHandEffector.positionOffset + r * leftHandPosRelToRightHand;
        fullBodyBipedIK.solver.leftHandEffector.positionOffset += leftHandTarget - fullBodyBipedIK.references.leftHand.position - fullBodyBipedIK.solver.leftHandEffector.positionOffset + r * leftHandOffset;
    }
    private void Read()
    {
        //왼손의 위치와 회전을 오른손에 상대적으로 기억합니다.
        leftHandPosRelToRightHand = fullBodyBipedIK.references.rightHand.InverseTransformPoint(fullBodyBipedIK.references.leftHand.position);
        leftHandRotRelToRightHand =Quaternion.Inverse(fullBodyBipedIK.references.rightHand.rotation) * fullBodyBipedIK.references.leftHand.rotation;
    }
    //1.이동(Move) 호출에서 에임 타겟을 가져오며, 이는 AimIK에서 사용됩니다.  
    //2.(Move는 캐릭터의 실제 움직임을 제어하는 CharacterController3rdPerson에 의해 호출됩니다.)
    //3.내가 짜논 이동 함수랑 커스텀해야 할듯 
    private void SetAimIK()
    {
        aimIk.solver.IKPosition = player.PlayerShootManager.AimPosition;
        aimIk.solver.target = player.PlayerShootManager.aimTransform ;
        aimIk.solver.Update();
    }
    private void FBBIK()
    {
        //오른손 회전 저장
        rightHandRotation = fullBodyBipedIK.references.rightHand.rotation;
        //손 위치를 계산 합니다. 
        Vector3 rightHandOffset = fullBodyBipedIK.references.rightHand.rotation * gunHolderOffset;
        fullBodyBipedIK.solver.rightHandEffector.positionOffset += rightHandOffset;
        if (recoil != null)
        {
            recoil.SetHandRotations(rightHandRotation * leftHandRotRelToRightHand, rightHandRotation);
        }
        //해당 계산 내용을 업데이트
        fullBodyBipedIK.solver.Update();

        // IK가 완료된 후 손 뼈를 회전시킵니다. 
        if (recoil != null)
        {
            fullBodyBipedIK.references.rightHand.rotation = recoil.rotationOffset * rightHandRotation;
            fullBodyBipedIK.references.leftHand.rotation = recoil.rotationOffset * rightHandRotation  * leftHandRotRelToRightHand;
        }
        else
        {
            fullBodyBipedIK.references.rightHand.rotation = rightHandRotation;
            fullBodyBipedIK.references.leftHand.rotation = rightHandRotation * leftHandRotRelToRightHand;
        }
    }
    private void HeadLookAt(Vector3 lookAtTarget)
    {
        Quaternion headRotationTarget = Quaternion.FromToRotation(fullBodyBipedIK.references.head.rotation * headLookAxis,lookAtTarget -fullBodyBipedIK.references.head.position);
        fullBodyBipedIK.references.head.rotation = Quaternion.Lerp(Quaternion.identity ,headRotationTarget,headLookWeight) * fullBodyBipedIK.references.head.rotation;
    }
    public void UpdateIK()
    {
        //캐릭터 처다보는 방향 
        Vector3 lookDirection = player.CameraController.transform.forward;
        //AimIK타겟 설정
        this.aimTarget = player.CameraController.transform.position + (lookDirection * 10f);
       // this.aimTarget = player.FinalDirection;
        Read();   // IK 절차, 카메라가 이동/회전된 후에 업데이트되도록 합니다.
                  // 현재 캐릭터의 포즈에서 무언가를 샘플링합니다.
        SetAimIK(); //에임 세팅
        FBBIK(); //AimIK가 해결되기 전에 왼손을 오른손에 상대적인 원래 위치로 되돌립니다.
        SetAimIK(); //에임 세팅
        HeadLookAt(aimTarget); //에임 타겟을 보도록 머리를 회전시킵니다.

    }


}
