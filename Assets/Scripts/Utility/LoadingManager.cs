using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PS
{
    public class LoadingManager : MonoBehaviour
    {
        public Image backgroundImage;
        public List<Sprite> backGroundSprites;
        public float fakeLodingTime;
        private void Start()
        {
           
            SetBackGroundImage();
            StartCoroutine(LoadSceneRoutine());
        }
        private void SetBackGroundImage()
        {
            
            int RandomValue =Random.Range(0, backGroundSprites.Count);
            Debug.Log(RandomValue);
            backgroundImage.sprite = backGroundSprites[RandomValue];
        }
        private IEnumerator LoadSceneRoutine()
        {
           AsyncOperation op =SceneManager.LoadSceneAsync(NextSceneManager.Instance.nextSceneName); //비동기로 불러오기 가능?
           op.allowSceneActivation = false; //씬 전환 90프로 정도에서 멈춤 -> fake로딩

           float timer = 0f;

            while (op.isDone ==false)
            {
                yield return null;

                if (op.progress < 0.9f) //씬에 로딩 90프로 까지 경우 
                {
                    Debug.Log(op.progress);               
                }
                else //씬 로딩이 90프로 이상 진행되었다면  나머지 10프로 동안 작업을 진행한다.
                {
                    timer += Time.unscaledDeltaTime;
                    if (timer >= fakeLodingTime)
                    { 
                        op.allowSceneActivation=true;
                        yield break;
                    }
                }
            }

        }

    }
}
