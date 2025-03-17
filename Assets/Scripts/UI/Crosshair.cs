using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [Header("정중앙 조준점")]
    public Image aimPointReticle; //조준 테두리
    [Header("플레이어 조준점")]
    public Image hitPointReticle; //조준 가운데
    [Header("조준점 UI 이동속도")]
    public float smoothTime = 10f;


    private Camera screenCamera;
    private RectTransform crossHairRectTransFrom;

    private Vector3 currentHitPoint; // 지금 내 마우스 에임
    private Vector3 targetPoint; // 내 화면 정중앙 조준 UI 
    private void Awake()
    {
        Init();
    }
    private void Update()
    {
        if (hitPointReticle.enabled == false) //마우스 에임 없으면 
        {
            return;
        }

        crossHairRectTransFrom.position = Vector3.SmoothDamp(crossHairRectTransFrom.position, targetPoint
            , ref currentHitPoint, smoothTime * Time.deltaTime);

    }
    private void Init()
    {
        screenCamera = Camera.main;
        crossHairRectTransFrom = hitPointReticle.GetComponent<RectTransform>();
    }
    public void SetActiveCrosshair(bool _active)
    {
        hitPointReticle.enabled = _active;
        aimPointReticle.enabled = _active;
    }
    public void UpdatePosition(Vector3 _worldPoint)
    {
        targetPoint = screenCamera.WorldToScreenPoint(_worldPoint);
        crossHairRectTransFrom = hitPointReticle.GetComponent<RectTransform>();
    }
}
