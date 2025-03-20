using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace PS
{
    public class LobbyUI : MonoBehaviour
    {
        public Canvas mainCanvas;
        public Canvas combatCanvas;
        public Button combatButton;
        public Image fadeImage;
        public float startAlphaValue = 1f;
        public float endAlphaValue =0f;
        public float fadeTime;

        private void Awake()
        {
            combatButton.onClick.AddListener(OnCombatButton);
            mainCanvas.gameObject.SetActive(true);
            combatCanvas.gameObject.SetActive(false);
        }

        private void OnCombatButton()
        {
            mainCanvas.gameObject.SetActive(false);
            combatCanvas.gameObject.SetActive(true);
            StartCoroutine(FadeRoutine(startAlphaValue,endAlphaValue));
        }
        private IEnumerator FadeRoutine(float _startValue ,float _endValue)
        {
            float time = 0f;
            Color aplhaColor =fadeImage.color;
            while (time < fadeTime)
            { 
                time += Time.deltaTime;

                aplhaColor.a = Mathf.Lerp(_startValue, _endValue,time/fadeTime);
                fadeImage.color = aplhaColor;
                yield return null;
            }
            aplhaColor.a = 0f;
            fadeImage.color = aplhaColor;
            fadeImage.gameObject.SetActive(false);
        }


    }
}
