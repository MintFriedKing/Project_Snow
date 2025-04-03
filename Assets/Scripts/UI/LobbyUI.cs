using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PS
{
    public class LobbyUI : MonoBehaviour
    {
        #region Canvas
        public Canvas mainCanvas;
        public Canvas combatCanvas;
        public Canvas characterSelectCanvas;
        public List<Canvas> canvasList;
        #endregion
        #region Button
        public Button combatButton;        // 
        public Button combatCanvasGoBackButton;
        public Button characterSelectCanvasGoBackButton;
        public Button soloPlayButton;
        public Button soloPlayStartButton;
        public Button chatOpenButton;
        public List<Button> homeButtons;
        #endregion
        public Image fadeImage;
        public float startAlphaValue = 1f;
        public float endAlphaValue =0f;
        public float fadeTime;
        public GameObject chatPanel;
       
        private void Awake()
        {
            Init();
        }
        private void Init()
        {
            
            mainCanvas.gameObject.SetActive(true);
            combatCanvas.gameObject.SetActive(false);
            characterSelectCanvas.gameObject.SetActive(false);
            soloPlayButton.onClick.AddListener(OnSoloPlayButton);
            combatButton.onClick.AddListener(OnCombatButton);
            foreach (Button homeButton in homeButtons)
            {
                homeButton.onClick.AddListener(OnHomeButton);

            }
            combatCanvasGoBackButton.onClick.AddListener(() => OnGobackButton(combatCanvasGoBackButton, combatCanvas));
            characterSelectCanvasGoBackButton.onClick.AddListener(() => OnGobackButton(characterSelectCanvasGoBackButton, characterSelectCanvas));
            soloPlayStartButton.onClick.AddListener(()=> OnNextSceneButton("SoloPlayInGame"));
            chatOpenButton.onClick.AddListener(OnChatPanel);
        }
        private void OnHomeButton()
        {
            foreach (Button button in homeButtons)
            {
                button.GetComponent<ButtonCanvasGroupController>().SetCanvasGroupAlpha();
            }

            foreach (Canvas canvas in canvasList)
            {
                canvas.gameObject.SetActive(false);
            }
           
            mainCanvas.gameObject.SetActive(true);
      
        }
        private void OnGobackButton(Button _goback,Canvas _canvas)
        {
            _goback.GetComponent<ButtonCanvasGroupController>().SetCanvasGroupAlpha();
            _canvas.gameObject.SetActive(false);   
        }
        private void OnNextSceneButton(string _sceneName)
        {
            NextSceneManager.Instance.nextSceneName = _sceneName;
            SceneManager.LoadScene("Loading");
        }
        private void OnSoloPlayButton()
        {
            characterSelectCanvas.gameObject.SetActive(true);
        }
        private void OnCombatButton()
        {       
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
        private void OnChatPanel()
        {
            if (chatPanel.gameObject.activeSelf == true)
            {
                chatPanel.gameObject.SetActive(false);
            }
            else 
            {
                chatPanel.gameObject.SetActive(true);
            }
        }
    }
}
