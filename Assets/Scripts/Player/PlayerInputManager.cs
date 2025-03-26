using PS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static PS.Player;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField]
    private Vector3 dir = Vector3.zero;
    [SerializeField, Header("마우스 x축 로테이션 값")]
    private float xRotation;
    [SerializeField, Header("마우스 y축 로테이션 값")]
    private float yRotation;
    [SerializeField, Header("x축 마우스 감도")]
    private float xSensitivity;
    [SerializeField, Header("y축 마우스 감도")]
    private float ySensitivity;
    private bool invertX;
    private int xInvertedValue;
    public Vector3 Dir { get { return dir; } }
    public float XRotation { get { return xRotation;} set { xRotation = value; } }
    public float YRotation { get { return yRotation; } set { yRotation = value; } }
    public float XSensitivity { get{ return xSensitivity; } }
    public float YSensitivity { get { return ySensitivity; } }

    public Player player;


    private void Start()
    {
        // 3인칭 게임에서 마우스를 아래로 내리면 위로 올라감 -> 즉 조작이 반대임 
        xInvertedValue = invertX ? -1 : 1;
       
    }
    private void Update()
    {
        if (SkillManager.Instance.IsLaserCahrge == true)
        {
            dir =Vector3.zero;
            return;
        }

        if (player != null && player.IsDodge == false && player.IsGround == true)
        {
            AimDownState();
            KeyBordInPut();
            MouseInPut();
        }
        
    }
    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            if (player.PlayerShootManager.gun.isReLoading == false)
            {
                player.SetPlayerState(PlayerState.COMBAT);
                Shoot();
            }
        }
    }

    #region 회피 입력
    private void Doge()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (dir.normalized != Vector3.zero && player.IsDodge == false)
            {
                player.DodgeIn();
            }
        }
    }
    #endregion 
    #region 달리기 입력
    private void Dash()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (dir.normalized != Vector3.zero 
                 && player.IsDodge == false && 
                player.IsGround == true && player.IsDash == false)
            {
                player.DashIn();
            }
        }
    }
    private void DashOut()
    {
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            player.DashOut();
        }
    }

    bool isQCharge = false;
    bool isQ = false;

    //눌럿을대
    private void OnSkillQEvent()
    {
        if (Input.GetKeyUp(KeyCode.Q) )
        {
            isQ = true;
            player.playerSkill.UseSkill();
        }
    }

    // 누르고 잇을대
    private void OnSkillCharge()
    {
        if(isQ == true)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //
            }
        }
    }
    // 댓을대
    // 인풋 관련해서는 눌럿을대, 댓을대, 누르고 잇을대 를 상태를 확인한다. 만약 누르고 잇는중 (차지, 푸쉬등등 일대 유요하게 사용)

    private void Skill()
    {
        if (Input.GetKeyUp(KeyCode.Q) && player.IsUseSkill ==false)
        {
           player.playerSkill.UseSkill();
           player.IsUseSkill = true;
           StartCoroutine(SkillManager.Instance.CoolDownRoutine(GameManager.Instance.SelectNumber-1));
        }
    }
    #endregion
    #region 사격 입력
    private void Shoot()
    {
        if (Input.GetMouseButton(0))
        {
            if(player.IsGround ==true && player.IsDash == false && player.IsDodge == false
                 &&  player.PlayerShootManager.gun.isReLoading == false)
            {            
                player.SetPlayerState(PlayerState.COMBAT);
                player.PlayerShootManager.Shoot();
                
            }
        }
    }
    private void ReLoad()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (player.IsGround == true && player.IsDash == false && player.IsDodge == false &&
                 player.PlayerShootManager.gun.isReLoading ==false && SkillManager.Instance.IsLaserCahrge == false)
            {
                player.PlayerShootManager.gun.Reload();
            }
        }
    }
    private void AimDownState()
    {
        if (player.IsGround == true && player.IsDash == false && player.IsDodge == false
            && SkillManager.Instance.IsLaserCahrge == false)
        {
            if (Input.GetMouseButton(1))
            {
                player.PlayerShootManager.isADS = true;

            }
            if (Input.GetMouseButtonUp(1))
            {
                player.PlayerShootManager.isADS = false;
            }

        }
    }
        #endregion
    private void KeyBordInPut()
    {
      dir.x = Input.GetAxis("Horizontal"); //수평 이동 
      dir.z = Input.GetAxis("Vertical"); // 수직 이동 
      dir.Normalize(); //정규화
      Doge(); // 회피
      Dash(); // 대쉬
      DashOut(); //대쉬 탈출
      Skill();
      ReLoad();
    }
    private void MouseInPut()
    {
        xRotation += Input.GetAxis("Mouse Y") * xSensitivity * xInvertedValue *Time.deltaTime;
        yRotation += Input.GetAxis("Mouse X") * ySensitivity * Time.deltaTime;

    }

}
