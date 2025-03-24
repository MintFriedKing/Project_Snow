using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace PS
{
    [Serializable]
    public class LobbyEventInformation
    {
        public Transform teleportTransform; //이동 위치
        public string animaionTriggerName; //애니메이션 트리거 
        
    }
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject lobbyCharacter;
        private bool isTuch;
        private int previousPositionNumber;
        private int previousAnimationNumber;
        [SerializeField]
        private float timer;
        public int selectValue;
        public static LobbyManager Instance;
        public List<LobbyEventInformation> lobbyEventInformations;
        public Animator animator;
        public bool IsTuch { get{ return isTuch; }set { isTuch = value; } }
        public GameObject LobbyCharacter{ get { return lobbyCharacter; }set { lobbyCharacter = value; } }
        public float animationChangeTime;

        


        private void Awake()
        {
            Instance = this;
            isTuch = true;
            lobbyCharacter.transform.position = lobbyEventInformations[0].teleportTransform.position;
            lobbyCharacter.transform.rotation = lobbyEventInformations[0].teleportTransform.rotation;
            animator.SetBool("IsFirstPosition",true);
         
            timer = animationChangeTime;
        }
        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                ChangeAnimation();
            }

            //if (Time.time >= animationChangeTime && selectValue == 0)
            //{
            //    SetNextAnimaion();
            //}
        }
     
        public void ChangeAnimation()
        {
        
            int nextAnimationValue;

            if (UnityEngine.Random.value < 0.8f)
            {
                nextAnimationValue = UnityEngine.Random.Range(0, 4);
                if (previousAnimationNumber == nextAnimationValue)
                {
                    nextAnimationValue = UnityEngine.Random.Range(0, 4);
                }
                Debug.Log(nextAnimationValue);
                animator.SetInteger("IdleValue", nextAnimationValue);
                timer = animationChangeTime;
                previousAnimationNumber = nextAnimationValue;
            }
            else 
            {
               
                timer = animationChangeTime;
                return;
            }
                      
            
        }

        private int GetRandomValue()
        {
            int value;
            do
            {
                value = UnityEngine.Random.Range(0, lobbyEventInformations.Count);
            }
            while (value == previousPositionNumber);
            previousPositionNumber = value;
            return value;

        }
       //private bool HasParameter(Animator animator, string paramName)
       // {
       //     foreach (AnimatorControllerParameter param in animator.parameters)
       //     {
       //         if (param.name == paramName)
       //         {
       //             return true;
       //         }
       //     }
       //     return false;
       //}
        public void OnTuched()
        {
             selectValue = GetRandomValue();
        
            Debug.Log(selectValue);
         
            //lobbyCharacter.transform.position = lobbyEventInformation[1].teleportTransform.position;
            lobbyCharacter.gameObject.SetActive(false);
            lobbyCharacter.gameObject.SetActive(true);
            lobbyCharacter.transform.SetParent(lobbyEventInformations[selectValue].teleportTransform);
            lobbyCharacter.transform.localPosition = Vector3.zero;
            lobbyCharacter.GetComponent<LobbyCharacter>().CreateSpawnParticle();
            lobbyCharacter.transform.rotation = lobbyEventInformations[selectValue].teleportTransform.rotation;
            animator.SetTrigger(lobbyEventInformations[selectValue].animaionTriggerName);
            if (selectValue == 0)
            {
                //if (HasParameter(animator, "IsFirstPosition") == false)
                //{
                //    Debug.LogError("애니메이터에 'IsFirstPosition' 파라미터가 존재하지 않습니다!");
                //}

                animator.SetBool("IsFirstPosition", true);
            }
            else
            {
                animator.SetBool("IsFirstPosition", false);
            }
        }
    }
}
