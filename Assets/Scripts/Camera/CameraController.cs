using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PS
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera;
        [SerializeField, Header("카메라 속도")]
        private float cameraFollowSpeed;
        [SerializeField, Header("x축 회전 최소값")]
        private float xRotationMin;
        [SerializeField, Header("x축 회전 최댓값")]
        private float xRotationMax;
        private float xRotationClamped;
        [SerializeField, Header("y축 회전 최소값")]
        private float yRotationMin;
        [SerializeField, Header("y축 회전 최댓값")]
        private float yRotationMax;
        private float yRotationClamped;


        [SerializeField, Header("카메라 부모 오브젝트와 카메라간 거리")]
        private Vector3 offset;
        [SerializeField, Header("카메라 높이(어깨높이)")]
        private float height;
        private Quaternion targetRotation;
        private Vector3 desiredPosition;
        [Header("카메라 타겟 오브젝트")]
        public Transform cameraTarget;
        [Header("플레이어 인풋 매니저")]
        public PlayerInputManager playerInputManager;

        public float adsFOV = 40f;
        public float normalFOV = 60f;
        public float smoothSpeed = 10f; //우클릭 전환 속도

        public Transform adsPosition; //줌땡길시 카메라 위치
        private  Vector3 normalPosition; //원래 위치

        
        public Vector3 Offset { get { return offset; } set { offset = value; } }
   

        private void Start()
        {
            Init();
        }
        private void Update()
        {
            SetCameraAimDownSight();
            SetCameraState();
        }
        private void LateUpdate()
        {
            SetCameraPositionAndRotation();
        }
        private void Init()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            normalPosition = this.gameObject.transform.position;

        }
        private void SetCameraState()
        {
            if (playerInputManager.XRotation > xRotationMax)
            {
                playerInputManager.XRotation = xRotationMax;
            }
            else if (playerInputManager.XRotation < xRotationMin)
            {
                playerInputManager.XRotation = xRotationMin;
            }

            //if (Player.Instance.playerState == PlayerState.COMBAT)
            //{

            //    if (playerInputManager.YRotation > yRotationMax)
            //    {
            //        playerInputManager.YRotation = yRotationMax;
            //    }
            //    else if (playerInputManager.YRotation < yRotationMin)
            //    {
            //        playerInputManager.YRotation = yRotationMin;
            //    }
            //    yRotationClamped = Mathf.Clamp(playerInputManager.YRotation, yRotationMin, yRotationMax);

            //}
            //else
            //{
            //    yRotationClamped =playerInputManager.YRotation;
            //}

            //Clamp 최소 최댓값 설정 
            xRotationClamped = Mathf.Clamp(playerInputManager.XRotation, xRotationMin, xRotationMax);
            targetRotation = Quaternion.Euler(xRotationClamped, playerInputManager.YRotation, 0f);
            //SetPositionAndRotation() 이 transfrom을 사용하여 수정하는것보다 최적하가 좋다는데 확인해볼것 
            //this.transform.SetPositionAndRotation(desiredPosition, targetRotation);
        }
        private void SetCameraPositionAndRotation()
        {

            float speed = cameraFollowSpeed * Time.deltaTime;
            desiredPosition = cameraTarget.position - targetRotation * offset + Vector3.up * height;
            this.transform.rotation = targetRotation;        
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, speed);

        }
        private void SetCameraAimDownSight()
        {
           Player player = GameManager.Instance.CurrentPlayer;
            
           if (player.PlayerShootManager.isADS == true)
            {
                Vector3 targetPosition = adsPosition.position;
                //적당한 위치로 카메라를 옮긴다.
                this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, Time.deltaTime * smoothSpeed);
                //FOV를 땡김
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, adsFOV, Time.deltaTime * smoothSpeed); //줌 땡긴다.
           }
            else
            {

                //적당한 위치로 카메라를 옮긴다.
                this.transform.position = Vector3.Lerp(this.transform.position, normalPosition, Time.deltaTime * smoothSpeed);
                //FOV를 땡김
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFOV, Time.deltaTime * smoothSpeed); //줌 땡긴다.
            }
        }
        public Quaternion YRotation => Quaternion.Euler(0f, playerInputManager.YRotation, 0f);
    }
}
