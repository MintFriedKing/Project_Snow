using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    public Image foreGroundImage;
    public Image backGroundImage;
    [SerializeField]
    private Vector3 offset;
 
    protected virtual void LateUpdate()
    {
        if (target != null)
        {
            SetHealthBarPosition();
        }
    }

    public virtual void SetHealthBarPosition()
    {
      
        Vector3 direction = (target.position - Camera.main.transform.position).normalized;
        // Vector3.Dot 두 지점에 내적을 구하는데 같은 방향이면 양수 반대면 음수를 반환하는듯? 
        bool isBehind = Vector3.Dot(direction, Camera.main.transform.forward) <= 0.0f;  //조건식에 맞으면 참일것이고 아니면 거짓  
        foreGroundImage.enabled = !isBehind;
        backGroundImage.enabled = !isBehind;
        //hp바 기준은 카메라상에 월드좌표로 변환된 타겟에 위치다.
        this.transform.position = Camera.main.WorldToScreenPoint(target.position + offset);

    }
    public virtual void SetImageAlphaValue(float _forGroundAlphaValue ,float _backGroundAlphaValue)
    {
        Color foreColor = foreGroundImage.color;
        foreColor.a = _forGroundAlphaValue;
        foreGroundImage.color = foreColor;

        Color backColor = backGroundImage.color;
        backColor.a = _backGroundAlphaValue;
        backGroundImage.color = backColor;
        
    }
    public virtual void SetHealthBar(float _percentage)
    {
        float parentWidth = GetComponent<RectTransform>().rect.width;
        float width = parentWidth * _percentage;
        foreGroundImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,width); //UI 요소의 크기를 현재 앵커(Anchor) 설정을 기준으로 조정하는 기능을 합니다.                                                                                                         //앵커를 손 안되면서 size 조정이 가능한듯?
    }

    
}
