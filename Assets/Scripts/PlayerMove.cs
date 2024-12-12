using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody plyaerRigidbody;
    [Header("애니메이션"), SerializeField]
    private Animator playerAnimator;
    private int horizontal;
    private int vertical;
    private Vector3 dir = Vector3.zero;
    [Header("이동 속도")]
    public float playerSpeed = 10f;
    [Header("대쉬")]
    public float dash = 5f;
    [Header("회전속도")]
    public float rotSpeed = 3f;
    [Header("공중여부")]
    public bool isGround = false;
    public LayerMask layer;


    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        PlayerInPut();
        CheckGround();
      
    }
    private void FixedUpdate() //물리 업데이트가 필요할때 많이 쓰는듯? 
    {
        PlayerRotation();

        plyaerRigidbody.MovePosition(this.gameObject.transform.position + dir * playerSpeed *Time.deltaTime);

    }
    public void Init()
    {
        plyaerRigidbody = this.GetComponent<Rigidbody>();
        playerAnimator = this.GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    private void PlayerInPut()
    {
        dir.x = Input.GetAxis("Horizontal"); //수평 이동 
        dir.z = Input.GetAxis("Vertical"); // 수직 이동 
        dir.Normalize(); //정규화
        //Mathf.Clamp01(value)  -> 주어진 값을 0 ~ 1로 제한  
        AnimatorValue(Mathf.Clamp01(Mathf.Abs(dir.x)), Mathf.Clamp01( Mathf.Abs(dir.z))); 


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Vector3 dashPower = this.transform.forward * dash;
            plyaerRigidbody.AddForce(dashPower,ForceMode.VelocityChange);
        } 
    }

    private void PlayerRotation()
    {
        if (dir != Vector3.zero) // 입력이 있을경우 
        {
            // 바로 뒤돌경우 회전이 즉각적으로 이루어지지 않음 그에따른 처리  -> 역방향 회전은 미리 돌려놓는다.
            //mathf는 양수 음수에 따라 1,0,-1 
            if (Mathf.Sign(transform.forward.x) != Mathf.Sign(dir.x) || Mathf.Sign(transform.forward.z) != Mathf.Sign(dir.z))
            {
               transform.Rotate(0, 1, 0);
            }
            //transform.forward = dir;  이 오브젝트에 앞방형은  dir 즉 입력한 방향이다. 
            //Vector3.Lerp 회전 보간이 들어가고 
            transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * rotSpeed);
           
        }
    }
    public void AnimatorValue(float horizontalMovement,float verticalMovemet)
    {
        float snappingHorizontal;
        float snappingVertical;

        #region Snapped Horizontal Value
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            snappingHorizontal = 0.5f;    // 0 ~ 5.5 준비동작  
        }
        else if (horizontalMovement > 0.55)
        {
            snappingHorizontal = 1f; //  0.55 이상은 본격적인 달리기 
        } // 여기까지는 앞방향으로 인한 처리다.
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            snappingHorizontal = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            snappingHorizontal = -1f;
        }
        else 
        {
            snappingHorizontal= 0f;
        }
        #endregion

        #region Snapped Vertical Value
        if (verticalMovemet > 0 && verticalMovemet < 0.55f)
        {
            snappingVertical = 0.5f;    // 0 ~ 5.5 준비동작  
        }
        else if (verticalMovemet > 0.55)
        {
            snappingVertical = 1f; //  0.55 이상은 본격적인 달리기 
        } // 여기까지는 앞방향으로 인한 처리다.
        else if (verticalMovemet < 0 && verticalMovemet > -0.55f)
        {
            snappingVertical = -0.5f;
        }
        else if (verticalMovemet < -0.55f)
        {
            snappingVertical = -1f;
        }
        else
        {
            snappingVertical = 0f;
        }
        #endregion 

        //horizontal 매개변수에 horizontalMovement 값을 설정하되, 애니메이션이 부드럽게 전환되도록 0.1초의 지연 시간을 둔다
        playerAnimator.SetFloat(horizontal, snappingHorizontal, 0f,Time.deltaTime);
        playerAnimator.SetFloat(vertical, snappingVertical, 0f, Time.deltaTime);
    }

 

    private void CheckGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + (Vector3.up * 0.2f), Vector3.down, out hit, 0.4f, layer))
        {
            isGround = true;
        }
        else 
        {
            isGround = false;
        }

    }   
}
