using BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

namespace PS
{
    [Serializable]
    public class PlayerInformation
    {
        public Player player;
        public Sprite  playerIcon; 
        public PlayerSkill skill;
        public Sprite skillIcon;
        public float hp; //체력 
        public float ammo; // 총알수 
        public float skillCoolTime;//스킬 쿨타임
        public float stamina;

    } 
    public class GameManager : MonoBehaviour
    {
        [SerializeField ,Header("player")]
        private Transform playerTransform;
        [SerializeField]
        private float timeLimit;
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
        [SerializeField]
        private LineRenderer linePath;   //길안내 선
        [SerializeField]
        private NavMeshAgent startPathPointnavMeshAgent; //안내 시작 위치
        [SerializeField]
        private NavMeshAgent endPathPointnavMeshAgent;
        [SerializeField]
        private Transform endPathPointTransform;

        [SerializeField]
        private float pathHeightOffset = 1.25f; // 라인 랜더러 높이 값
        [SerializeField]
        private float pathUpdateSpeed = 0.25f;// 길안내 선 업데이트 주기                                    
        //private NavMeshTriangulation triangulation; // 네브매쉬 지형데이터를 가져올 변수 


        public static GameManager Instance;
        public List<Transform> pathTransforms; 
        public float currentGameTime;
        private Coroutine drawPathCoroutine;
        public int SelectNumber { get { return selectNumber; } }
        public int PreviousSlectNumber { get { return previousSlectNumber; } }
        public Player CurrentPlayer { get { return currentPlayer; } }
        public Transform CameraFollowTransform { get { return cameraFollowTransform; } }
        public Transform PlayerTransform { get { return playerTransform; } }
        public PlayerInputManager PlayerInputManager { get { return playerInputManager; } set { playerInputManager = value; } }
        public float TimeLimit { get { return timeLimit; } }
        private void Awake()
        {
            Instance = this;
            currentGameTime = TimeLimit * 60f;
            CharacterInit();
            for (int i = 0; i < players.Count; i++)
            {
                SkillManager.Instance.SkillCoolTimes[i] = players[i].skillCoolTime;
            }
               
        }
        private void Start()
        {
            //InvokeRepeating(nameof(DrawPath),0f,pathUpdateSpeed);
            DrawPath();
        }
        public void Update()
        {
           
            if (currentGameTime > 0)
            {
                currentGameTime -= Time.deltaTime;
            }
            if (currentGameTime < 0)
            {
                Time.timeScale = 0;
            }

            if (SkillManager.Instance.IsHealing == false &&
                 SkillManager.Instance.IsLaserCahrge == false 
                 && SkillManager.Instance.IsShield ==false)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1) 
                    && GameManager.Instance.players[0].player.playerState != Player.PlayerState.DIE)
                {
                   
                    previousSlectNumber = selectNumber;
                    selectNumber = 1;
                    SelectCharacter(selectNumber);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2)
                    && GameManager.Instance.players[1].player.playerState != Player.PlayerState.DIE)
                {
                    previousSlectNumber = selectNumber;
                    selectNumber = 2;
                    SelectCharacter(selectNumber);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3)
                    && GameManager.Instance.players[2].player.playerState != Player.PlayerState.DIE)
                {
                    previousSlectNumber = selectNumber;
                    selectNumber = 3;
                    SelectCharacter(selectNumber);
                }
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
                PlayerInformation.skillCoolTime = PlayerInformation.player.playerSkill.skillCoolTime;
                PlayerInformation.stamina = PlayerInformation.player.MaxStemina; //달릴때 쓰는 스테미나 
            }
         
            SelectCharacter(selectNumber);
            players[selectNumber - 1].player.gameObject.SetActive(true);
            currentPlayer = players[selectNumber - 1].player;
            playerInputManager.player = currentPlayer;
        }
        private void SwapCharacter(int _selectNumber)
        {       
            if (previousSlectNumber == _selectNumber)
            {        
                return;             
            }
            else
            {
                previousTransform = players[previousSlectNumber - 1].player.transform;
                currentTransform = previousTransform;
                previousRotation = players[previousSlectNumber - 1].player.transform.rotation;
                currentRotation = previousRotation;
                currentPlayer = players[_selectNumber - 1].player;
           
                for (int i = 0; i < players.Count; i++) 
                {
                    if (i == (_selectNumber - 1)) 
                    {
                                                                                                            
                        playerInputManager.player = currentPlayer;                   
                        players[_selectNumber - 1].player.gameObject.transform.position = currentTransform.position;  
                        players[_selectNumber - 1].player.transform.rotation = currentRotation;                
                        players[_selectNumber - 1].player.gameObject.SetActive(true); 
                        swapPaticleSystem.Play();
                      
                    }
                    else
                    {
                        players[i].player.gameObject.SetActive(false);
                    }               
                }
            }     
                UIManager.Instance.playerStatusIcon.sprite = players[selectNumber - 1].playerIcon;
                UIManager.Instance.ChangeCharacterList();
                UIManager.Instance.ChangeSkillSprite();
                UIManager.Instance.InitStatusBar();
            
        }
        public void SelectCharacter(int _SelectNumber)
        {
            SwapCharacter(_SelectNumber);
        }
        public void DieCharacterSwap()
        {
            if (OnCheckAllDie() == false)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].player.playerState != Player.PlayerState.DIE)
                    {
                        selectNumber = i + 1;
                        SelectCharacter(i + 1);
                        break;
                    }
                }
            }
            else
            {
                UIManager.Instance.mainCanvas.gameObject.SetActive(false);
                UIManager.Instance.defeatCanvas.gameObject.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None; // 커서 잠금 해제

            }
        }
        public bool OnCheckAllDie()
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].player.playerState != Player.PlayerState.DIE)
                {
                    return false;
                   
                }
            }

            return true;
        }

        private IEnumerator DrawPathRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(pathUpdateSpeed);

            NavMeshPath path = new NavMeshPath();

            while (pathTransforms != null)
            {
                if (NavMesh.CalculatePath(pathTransforms[0].position, pathTransforms[2].position
                                                    , NavMesh.AllAreas, path))
                {
                    linePath.positionCount = path.corners.Length;

                    for (int i = 0; i < path.corners.Length; i++)
                    {
                        linePath.SetPosition(i, path.corners[i] + Vector3.up * pathHeightOffset);
                    }
                }
                else 
                {
                    Debug.Log($"Unable to calculate a path on the navmesh betwen {pathTransforms[0].position} and {pathTransforms[2].position}");
                }
                yield return wait;
            }     
        }

        public GuideTrail trail;
        private void DrawPath()
        {
            if (startPathPointnavMeshAgent == null || endPathPointTransform == null || linePath == null)
            {          
                return;
            }  
            NavMeshHit hit;
            // End 위치 확인 및 보정
            if (NavMesh.SamplePosition(endPathPointTransform.position, out hit, 1.0f, NavMesh.AllAreas))
            {
                endPathPointTransform.position = hit.position;
                Debug.Log($"End 위치 보정 완료! 새로운 위치: {hit.position}");
            }
            else
            {
                Debug.LogError("End 위치가 NavMesh 위에 없음!");
            }

            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(startPathPointnavMeshAgent.transform.position, 
                endPathPointTransform.position, NavMesh.AllAreas, path))
            {
                linePath.positionCount = path.corners.Length;
                Debug.Log(linePath.positionCount);

                for (int i = 0; i < path.corners.Length; i++)
                {
                    linePath.SetPosition(i, path.corners[i] + Vector3.up * pathHeightOffset);
                
                }
               
            }
            else
            {
                Debug.LogError("NavMesh.CalculatePath() 실패! start, end 위치 확인 필요");
                Debug.LogError($"Start Position: {startPathPointnavMeshAgent.transform.position}");
                Debug.LogError($"End Position: {endPathPointTransform.position}");

            }

            trail.Setting();
        }

    }
}
