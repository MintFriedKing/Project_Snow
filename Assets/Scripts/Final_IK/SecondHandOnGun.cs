using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondHandOnGun : MonoBehaviour
{

    public AimIK aimIk;
    public LimbIK leftArmIK;
    public Transform leftHand;
    public Transform rightHand;
    public FullBodyBipedIK fullBodyBipedIK;

    public Vector3 leftHandPositionOffset; //왼손 위치
    public Vector3 leftHandRotationOffset; // 왼손 로테이션 

    private Vector3 leftHandPosRelToRight;// 오른손에 상대적인 위치인 왼손이라는데 대체 뭐라는건지 모르겠다.
    private Quaternion leftHandRotRelToRight;
    //private IKEffector leftHand { get {return fullBodyBipedIK.solver.leftHandEffector; } }
    //private IKEffector rightHand { get {return fullBodyBipedIK.solver.rightHandEffector; } }
    private void Awake()
    {
        Init();
    }
    private void LateUpdate()
    {
        UpdateHand();
    }

    private void Init()
    { 
        aimIk = this.GetComponent<AimIK>();
        fullBodyBipedIK = this.GetComponent<FullBodyBipedIK>();
        leftArmIK = this.GetComponent<LimbIK>();
        //사용자가 직접 업데이트 할려면 비활성화 하라고 명시됨
        aimIk.enabled =false;
        leftArmIK.enabled =false;
        fullBodyBipedIK.enabled = false;

    }
    private void UpdateHand()
    {
        //InverseTransformDirection는 월드 좌표를 로컬 좌표로 변환?
        //왼손의 위치와 회전을 오른손에 상대적인 값으로 저장하되, 애니메이션된 상태로 저장한다.
        //인간은 대칭이라 이렇게 하나 ? 
        leftHandPosRelToRight = rightHand.InverseTransformDirection(leftHand.position);
        leftHandRotRelToRight = Quaternion.Inverse(rightHand.rotation) *leftHand.rotation;

        aimIk.solver.Update(); //오른팔 업데이트 
        //AimIK가 오른팔을 이동시켰으므로, 손은 이전에 오른손에 상대적으로 있었던 상태로 되돌려야 합니다 (위의 과정을 역으로 수행)."
        leftArmIK.solver.IKPosition = rightHand.TransformPoint(leftHandPosRelToRight + leftHandPositionOffset);
        leftArmIK.solver.IKRotation = rightHand.rotation * Quaternion.Euler(leftHandRotationOffset) * leftHandRotRelToRight;

        leftArmIK.solver.Update(); //왼팔 업데이트 
        //le

    }
   //private void IkStateUpdate()
    //{
    //    //find out how to the left hand is positioned relative to the right hand
    //    // 왼손이 오른손에 비해 어떻게 위치해 있는지 알수 있다?


    //    Vector3 toLeftHend = leftHand.bone.position - rightHand.bone.position;    //왼손과 오른손 사이의 월드 좌표에서의 위치 차이를 계산합니다.
    //    Vector3 toLeftHandRelative = rightHand.bone.InverseTransformDirection(toLeftHend); //InverseTransformDirection을 사용하여 오른손을 기준으로 한 로컬 공간의 상대 위치를 구합니다.
    //    aimIk.solver.Update();

    //    //Position the left hand on the gun
    //    //왼손 위치를 오른손에 위치시키다.
    //    leftHand.position = rightHand.bone.position + rightHand.bone.TransformDirection(toLeftHandRelative); //오른손의 위치와 상대 위치(toLeftHandRelative)를 사용해 왼손을 총의 적절한 위치에 배치합니다.
    //    leftHand.positionWeight = 0.9f; //positionWeight = 1f는 왼손의 위치 설정이 100% 적용되도록 합니다.

    //    //making sure the right hand won't budge during solving
    //    //오른손이 해결 과정에서 움직이지 않도록 확인하기
    //    rightHand.position = rightHand.bone.position; //오른손의 위치를 현재 위치로 고정하여 해결 과정에서 움직이지 않도록 설정합니다
    //    rightHand.positionWeight = 1f;
    //    fullBodyBipedIK.solver.GetLimbMapping(FullBodyBipedChain.RightArm).maintainRotationWeight =1f; //maintainRotationWeight = 1f는 오른팔의 회전을 유지하도록 설정합니다.


    //    fullBodyBipedIK.solver.Update();
    //    //Vector3 toRightHend =
    //}

}
