using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PS;
using System.ComponentModel;

public class PlayerAnimationManager : MonoBehaviour
{
    [Header("애니메이션"), SerializeField]
    private Animator playerAnimator;
    private int horizontal;
    private int vertical;
    private Player player;
    public Animator PlayerAnimator { get { return playerAnimator; } }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (playerAnimator == null)
        {
            playerAnimator = this.GetComponent<Animator>();
        }


        UnityEditor.EditorUtility.SetDirty(this);

#endif 
    }
    private void Awake()
    {
        Init();
    }
    public void SetMovementAnimatorValue(float _horizontalMovement, float _verticalMovemet)
    {
   
        float snappingHorizontal= 0f;
        float snappingVertical=0f;

        #region Snapped Horizontal Value
        if (_horizontalMovement > 0 && _horizontalMovement < 0.55f)
        {
            snappingHorizontal = 0.5f;    // 0 ~ 5.5 준비동작  
        }
        else if (_horizontalMovement > 0.55)
        {
            snappingHorizontal = 1f; //  0.55 이상은 본격적인 달리기 
        } // 여기까지는 앞방향으로 인한 처리다.
        else if (_horizontalMovement < 0 && _horizontalMovement > -0.55f)
        {
            snappingHorizontal = -0.5f;
        }
        else if (_horizontalMovement < -0.55f)
        {
            snappingHorizontal = -1f;
        }
        else
        {
            snappingHorizontal = 0f;
        }
        #endregion

        #region Snapped Vertical Value
        if (_verticalMovemet > 0 && _verticalMovemet < 0.55f)
        {
            snappingVertical = 0.5f;    // 0 ~ 5.5 준비동작  
        }
        else if (_verticalMovemet > 0.55)
        {
            snappingVertical = 1f; //  0.55 이상은 본격적인 달리기 
        } // 여기까지는 앞방향으로 인한 처리다.
        else if (_verticalMovemet < 0 && _verticalMovemet > -0.55f)
        {
            snappingVertical = -0.5f;
        }
        else if (_verticalMovemet < -0.55f)
        {
            snappingVertical = -1f;
        }
        else
        {
            snappingVertical = 0f;
        }
        #endregion 

        //horizontal 매개변수에 horizontalMovement 값을 설정하되, 애니메이션이 부드럽게 전환되도록 0.1초의 지연 시간을 둔다   
        playerAnimator.SetFloat(horizontal, snappingHorizontal, 0.1f, Time.deltaTime);
        playerAnimator.SetFloat(vertical, snappingVertical, 0.1f, Time.deltaTime);
    

    }
    private void Init()
    {
        player = this.GetComponent<Player>();
        playerAnimator = this.GetComponent<Animator>();
        // 상반신 애니메이터를 활성화 
        playerAnimator.SetLayerWeight(1, 0f);
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }
    
    public void Dodge()
    {
        playerAnimator.SetTrigger("Dodge");
    }
    public void InAir(bool _isGround)
    {
         
        playerAnimator.SetBool("IsGround",_isGround);
       
    }
    public void Dash(bool _isDash)
    {
        playerAnimator.SetBool("IsDash", _isDash);
    }
    public void Combat(bool _isCombat)
    {
        playerAnimator.SetBool("IsCombat", _isCombat);
    }
    public void Sheild()
    {
        playerAnimator.SetTrigger("Shield");
    }
    public void LaserBeam()
    {
        playerAnimator.SetTrigger("LaserBeam");
    }
    public void Heal()
    {
        playerAnimator.SetBool("IsCasting",true);
        playerAnimator.SetTrigger("Heal");
    }
    public void ReLoad()
    {
        playerAnimator.SetTrigger("ReLoad");
    }
}

