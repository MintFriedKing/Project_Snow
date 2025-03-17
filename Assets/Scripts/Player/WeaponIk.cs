using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HumanBone 
{
    public HumanBodyBones humanBodyBone;
    public float weight = 1.0f;
}
public class WeaponIk : MonoBehaviour
{
    private PlayerShootManager playershootManager;
    public Vector3 targetPosition;
    public Transform aimTransform;
    public Transform bone;
    public int iterations = 10;
    [SerializeField,Range(0,1)]
    public float weight = 1.0f;

    public float angleLimit = 90f;  // 허리가 꺽이는거 방지 
    public float distanceLimit = 1.5f ; //거리 제한

    public HumanBone[] humanBodyBones;
    public Transform[] boneTransforms;

    public HumanBone[] originalHumanBone;
    public Transform[] originalboneTransforms;


    private void Start()
    {
        Init();
        //this.GetComponent<PlayerShootManager>().AimPosition;

    }
    private void LateUpdate()
    {
       
        UpdateWeaponeIK();

    }
     public void Init()
     { 
        Animator animator =this.GetComponent<Animator>();
        boneTransforms = new Transform[humanBodyBones.Length];
        playershootManager = this.GetComponent<PlayerShootManager>();
        for (int i = 0; i < boneTransforms.Length; i++)
        { 
            boneTransforms[i] = animator.GetBoneTransform(humanBodyBones[i].humanBodyBone);
        }

        originalboneTransforms = boneTransforms;
        originalHumanBone = humanBodyBones;

     }
    public void Reset()
    { 
        boneTransforms = originalboneTransforms;
        humanBodyBones = originalHumanBone;
    }
    private void UpdateWeaponeIK()
    {
        if (aimTransform == null) 
        {
            return;
        }
        if (targetPosition == null)
        {
            return;
        }
       // Vector3 targetPostion = playershootManager.AimPosition;
       Vector3 targetPostion = GetTatgetPosition();

        for (int i = 0; i < iterations; i++)
        {
            for (int j = 0; j < boneTransforms.Length; j++)
            {
                Transform bone = boneTransforms[j];
                float bonWeight = humanBodyBones[j].weight * weight;
                AimAtTarget(bone, targetPostion, bonWeight);
            }
        }
    }
    private Vector3 GetTatgetPosition()
    {
        Vector3 targetDirection = playershootManager.AimPosition - aimTransform.position;
        //Vector3 targetDirection =aimTransform.position;
        Vector3 aimDirection = aimTransform.forward;
        float blendOut = 0f;
        float targetAngle = Vector3.Angle(targetDirection, aimDirection);
        if (targetAngle > angleLimit)
        {
            blendOut += (targetAngle - angleLimit) / 50f;
        }
        float targetDistance = targetDirection.magnitude;
        if (targetDistance < distanceLimit)
        {
            blendOut += distanceLimit - targetDistance;
        }

        Vector3 direction = Vector3.Slerp(targetDirection, aimDirection, blendOut);
      
        return aimTransform.position + direction;
    }
    private void AimAtTarget(Transform _bone, Vector3 _targetPosition,float _weight)
    {
        Vector3 aimDirection = aimTransform.forward; // 에임 잡는 방향 
        Vector3 targetDirection = _targetPosition - aimTransform.position; //에임 위치부터 타겟 위치의 방향 
        Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
        Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity ,aimTowards, _weight);
        _bone.rotation = aimTowards * _bone.rotation;
        //Debug.Log($"{_bone.name}:{_bone.rotation} ");

    }
    public void SetAimTransform(Transform _aimTransform)
    {
        aimTransform = _aimTransform;
    }
    public void SetTargetPosition(Vector3 _target)
    { 
        targetPosition = _target;
    }
    
}
