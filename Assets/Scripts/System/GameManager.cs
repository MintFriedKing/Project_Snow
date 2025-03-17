using BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    [Serializable]
    public class PlayerInformation
    {
        public Player player;
        public PlayerSkill skill;
        public float hp; //체력 
        public float ammo; // 총알수 
        public float skillCoolTime;

    } 
    public class GameManager : MonoBehaviour
    {
        [SerializeField ,Header("player")]
        private Transform playerTransform;
        [SerializeField]
        public List<PlayerInformation> players;
        public bool isChange;
        public ParticleSystem swapPaticleSystem;

        private Player currentPlayer;
        [SerializeField]
        private PlayerInputManager playerInputManager;
        [SerializeField]
        private CameraController cameraController;
        [SerializeField]
        private Transform cameraFollowTransform;
        [SerializeField]
        private int selectNumber;               // 현재 선택 번호 
        private int previousSlectNumber;     // 이전 선택 번호 
        private Transform previousTransform;  // 이전 캐릭터 위치
        private Transform currentTransform;   // 현재 캐릭터 위치
        private Quaternion previousRotation;
        private Quaternion currentRotation;

        public static GameManager Instance;

        public int SelectNumber { get { return selectNumber; } }
        public Player CurrentPlayer { get { return currentPlayer; } }
        public Transform CameraFollowTransform { get { return cameraFollowTransform; } }
        public Transform PlayerTransform { get { return playerTransform; } }
        public PlayerInputManager PlayerInputManager { get { return playerInputManager; } set { playerInputManager = value; } }
        private void Awake()
        {    
            CharacterInit();
            players[selectNumber -1].player.gameObject.SetActive(true);
            currentPlayer = players[selectNumber - 1].player;
            playerInputManager.player = currentPlayer;
            Instance = this;
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                previousSlectNumber = selectNumber;
                selectNumber = 1;
                CharacterSelect(selectNumber);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                previousSlectNumber = selectNumber;
                selectNumber = 2;
                CharacterSelect(selectNumber);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                previousSlectNumber = selectNumber;
                selectNumber = 3;
                CharacterSelect(selectNumber);
            }
        }
        public void CharacterInit()
        {
            previousSlectNumber = selectNumber;
            foreach (var PlayerInformation in players)
            {
                PlayerInformation.player.Init(); // 각 캐릭터를 초기화
                PlayerInformation.hp = PlayerInformation.player.PlayerHealth.Maxhealth; // hp 
                PlayerInformation.ammo = PlayerInformation.player.PlayerShootManager.gun.MaxCapacity; // 총알
                PlayerInformation.skill = PlayerInformation.player.playerSkill;
            }
            //previousTransform = players[selectNumber - 1].player.gameObject.transform;
            //CharacterSelect(selectNumber, previousTransform);
            CharacterSelect(selectNumber);
        }
        private void CharacterSwap(int _selectNumber)
        {
           
            if (previousSlectNumber == _selectNumber) // 이전 과 지금이 같은가? 
            {
                return;             
            }
            else
            {
                previousTransform = players[previousSlectNumber - 1].player.transform; //이전 캐릭터의 위치를 대입하고
                currentTransform = previousTransform; //현재에 적용
         
                previousRotation = players[previousSlectNumber - 1].player.transform.rotation;
                currentRotation = previousRotation;

                currentPlayer = players[_selectNumber - 1].player;
           

                //1.다르다면 교체해야한다.
                for (int i = 0; i < players.Count; i++) //
                {
                    if (i == (_selectNumber - 1)) //선택한 캐릭터의 경우 
                    {
                        //cameraController.cameraTarget = players[_selectNumber - 1].cameraTargetTransform;//카메라 목표 교체해주고                                                                                             
                        playerInputManager.player = currentPlayer;
                       // cameraController.playerInputManager = players[_selectNumber - 1].player.PlayerInputManager;
                        players[_selectNumber - 1].player.gameObject.transform.position = currentTransform.position;  //위치를 정하고
                        players[_selectNumber - 1].player.transform.rotation = currentRotation;             //회전을 정하고      
                        players[_selectNumber - 1].player.gameObject.SetActive(true); //활성화 
                        swapPaticleSystem.Play();
                        //players[_selectNumber - 1].player.PlayerAnimationManager.SetMovementAnimatorValue(Mathf.Clamp01(Mathf.Abs(playerInputManager.Dir.x)), Mathf.Clamp01(Mathf.Abs(playerInputManager.Dir.z)));

                    }
                    else
                    {
                        players[i].player.gameObject.SetActive(false);
                    }
                    //players[_selectNumber - 1].player.PlayerAnimationManager.InAir(false);
                }

            }
         
        }
        private void CharacterSelect(int _SelectNumber)
        {
            CharacterSwap(_SelectNumber);
        }

    }
}
