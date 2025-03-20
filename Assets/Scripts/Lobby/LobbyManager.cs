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
        public void OnTuched()
        {
             selectValue = GetRandomValue();
        
            Debug.Log(selectValue);
                  
            lobbyCharacter.gameObject.SetActive(false);
            lobbyCharacter.gameObject.SetActive(true);
            lobbyCharacter.transform.SetParent(lobbyEventInformations[selectValue].teleportTransform);
            lobbyCharacter.transform.localPosition = Vector3.zero;
            lobbyCharacter.GetComponent<LobbyCharacter>().CreateSpawnParticle();
            lobbyCharacter.transform.rotation = lobbyEventInformations[selectValue].teleportTransform.rotation;
            animator.SetTrigger(lobbyEventInformations[selectValue].animaionTriggerName);
            if (selectValue == 0)
            {
                
                animator.SetBool("IsFirstPosition", true);
            }
            else
            {
                animator.SetBool("IsFirstPosition", false);
            }
        }
    }
}
