using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBar : HealthBar
{
    public RectTransform playerHpRectTransform;
  
    protected override void LateUpdate()
    { 
        
    }
    public override void SetHealthBarPosition()
    {

    }
    public override void SetHealthBar(float _percentage)
    {
        //playerHpRectTransform = GetComponent<RectTransform>().rect.width;
        float width = playerHpRectTransform.rect.width * _percentage;
        foreGroundImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width); //UI 요소의 크기를 현재 앵커(Anchor) 설정을 기준으로 조정하는 기능을 합니다.                                                                                                   //앵커를 손 안되면서 size 조정이 가능한듯?
    }

}
