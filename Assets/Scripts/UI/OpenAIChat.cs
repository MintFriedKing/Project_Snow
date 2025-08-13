using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;




namespace PS
{
    [Serializable]
    public class OpenAIRequest
    {
        public string model = "gpt-4o";
        public string input;
        public bool stream = false;
        public string instructions = "You are a sweet woman. Your name is Lee Joo-hee and your nationality is South Korea. We are in a very close relationship. Also, we have known each other for a long time and trust each other.";
         

    }
    [Serializable]
    public class ContentItem
    {
        public string text;
    }

    [Serializable]
    public class ResponseOutput
    {
        public List<ContentItem> content;
    }
    [Serializable]
    public class OpenAIResponse
    {
        public string id;
        public List<ResponseOutput> output;
    }
    public class OpenAIChat : MonoBehaviour
    {
        [SerializeField]
        private string apiKey;
        [SerializeField]
        private string apiUrl;

        public InputField messageInPutField;
        public Button sendButton;
        public Transform  chatprefabTransform;
        public GameObject sendPrefab;
        public GameObject recivePrefab;
        public ScrollRect chatScrollRect;
        public Scrollbar chatScrollbar;
        // 채팅 메시지 보내기
        private void Awake()
        {
            sendButton.onClick.AddListener(SendChatMessage);
        }
        public void Update()
        {
            if (this.gameObject.activeSelf == true)
            {
                chatScrollRect.verticalNormalizedPosition = 0f;
                chatScrollbar.value = 0f;
            }
        }
        public void SendChatMessage()
        {
            if (string.IsNullOrEmpty(messageInPutField.text)) return;

            string userMessage = messageInPutField.text;
            // 입력 필드 초기화
            messageInPutField.text = "";

            GameObject sendMessage = Instantiate(sendPrefab, chatprefabTransform);
            sendMessage.GetComponent<ChatPanel>().messagetext.text = userMessage;
            
            // API 요청 코루틴 시작
            StartCoroutine(SendOpenAIRequest(userMessage));
        }
        // 요청 코루틴 
        private IEnumerator SendOpenAIRequest(string userMessage)
        {
            // 요청 본문 구성
            OpenAIRequest requestBody = new OpenAIRequest
            {
                input = userMessage
               
            };
            string jsonBody = JsonUtility.ToJson(requestBody);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

            // 웹 요청 설정
            using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + apiKey);

                // 요청 전송
                yield return request.SendWebRequest();

                // 응답 처리
                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseJson = request.downloadHandler.text;
                    OpenAIResponse response = JsonUtility.FromJson<OpenAIResponse>(responseJson);

                    // 응답 처리 및 채팅 로그에 추가
                    if (response.output != null && response.output.Count > 0
                        && response.output[0].content != null
                        && response.output[0].content.Count > 0)
                    {
                        string aiResponse = response.output[0].content[0].text;

                        Debug.Log(aiResponse);
                        //chatLogText.text += $"AI: {aiResponse}\n";
                        GameObject reciveMessage = Instantiate(recivePrefab, chatprefabTransform);
                        reciveMessage.GetComponent<ChatPanel>().messagetext.text = $"{aiResponse}";
                      
                    }
                }
                else
                {
                    Debug.LogError("API 요청 실패: " + request.error);
                    //chatLogText.text += $"Error: {request.error}\n";
                }
            }
           

        }

    }
}
