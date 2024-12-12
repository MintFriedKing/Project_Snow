using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    public Transform orienTation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    [Header("카메라")]
    public float rotationSpeed;
    public enum CameraStyle 
    {
        BASIC,
        COMBAT,
        TOPDOWN
    }
    public CameraStyle currentCameraStyle;
    public Transform combatLookAt;
    [Header("시점별 시네머신 카메라")]
    public GameObject thirdPersonCam;
    public GameObject combatCam;
    public GameObject topdownCam;


    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchCameraStyle(CameraStyle.BASIC);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchCameraStyle(CameraStyle.COMBAT);
        }
         if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchCameraStyle(CameraStyle.TOPDOWN);
        }
        //rotate orientation
        SetViewDirection();
        //rotate player object
        CharacterInput();
    }

    public void SetViewDirection()
    {
        // 내 위치 - 다른 오브젝트 위치  ==  두 오브젝트 위치간에 거리를 알수 있고 정규화하면 방향도 구할수 있단걸 기억할것 
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orienTation.forward = viewDir.normalized;
    }
    public void CharacterInput()
    {
        if (currentCameraStyle == CameraStyle.BASIC)
        {
            float horizontalInPut = Input.GetAxis("Horizontal"); // 원쪽 , 오른쪽
            float verticalInPut = Input.GetAxis("Vertical"); //앞,뒤
            Vector3 inputDir = orienTation.forward * verticalInPut + orienTation.right * horizontalInPut;

            if (inputDir != Vector3.zero)  // 입력이 있을경우 
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }
        }
        else if(currentCameraStyle ==CameraStyle.COMBAT)
        {
            Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orienTation.forward = dirToCombatLookAt.normalized;
            playerObj.forward = dirToCombatLookAt.normalized;
        }

        
    }

    public void SwitchCameraStyle(CameraStyle newCameraStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);
        topdownCam.SetActive(false);

        switch (newCameraStyle)
        { 
            case CameraStyle.BASIC:
                thirdPersonCam.SetActive (true);    
                break;
            case CameraStyle.COMBAT:
                combatCam.SetActive(true);
                break;
            case CameraStyle.TOPDOWN:
                topdownCam.SetActive(true);
                break;
        }

        currentCameraStyle = newCameraStyle;
    }
  
}
//1.나중에 무브먼트 클래스 하나더 
//2.오브젝트 안에 카메라가 있으면  따라가잖아 -> 애초에 이동을 구현하고 카메라를 보러 가야지 임마 -> 순서가 바뀜
//3. 무브먼트 먼저  -> 그다음 카메라 구현 -> 
//4. 이거 버려  ThirdPersonCam  -> 카메라를 번경하는 스크립트임 