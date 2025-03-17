using RootMotion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollison : MonoBehaviour
{
    public LayerMask hitMask;
    public float distance;
    public float minDistance = 1.0f;
    public float maxDistance = 4.0f;
    public float smoth = 10f;
    public Vector3 dollyDirectionAdjust;

    private CameraController cameraController;
    private Vector3 dollyDirection;

    private void Awake()
    {
        Init();
    }
    private void Update()
    {
        UpDateCameraCollison();
    }

    private void Init()
    {
        //normalized는 벡터를 **단위 벡터(Unit Vector)**로 변환합니다.
        //즉, 방향은 유지하되 길이를 1로 맞춘 벡터를 반환합니다.
        //magnitude는 벡터의 **크기(길이)**를 반환합니다.
        //즉, (x, y, z) 좌표값을 이용하여 원점(0, 0, 0)으로부터의 거리(벡터의 크기)를 계산합니다. 
        dollyDirection = this.transform.localPosition.normalized; //카메라 방향
        distance = this.transform.localPosition.magnitude;
        cameraController = this.GetComponentInParent<CameraController>();
    }
    private void UpDateCameraCollison()
    {
        Vector3 desirdCameraPosition = transform.parent.TransformPoint(dollyDirection * maxDistance);
        RaycastHit hit;
        //내가 지정한 위치와 대상간의 위치에 선을 교차하는 충돌체가 있을경우 true를 반환한다.
        //이경우는 카메라와 캐릭터인듯?
        //if(Physics.Raycast(this.transform.position, this.transform.forward , out hit,))

        if (Physics.Linecast(transform.parent.position, desirdCameraPosition, out hit, hitMask))
        {

            distance = Mathf.Clamp(hit.distance * 0.9f, minDistance, maxDistance);
        }
        else
        {

            distance = maxDistance;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDirection * distance, Time.deltaTime * smoth);
    }
}
