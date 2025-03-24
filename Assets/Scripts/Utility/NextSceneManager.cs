using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class NextSceneManager : MonoBehaviour
    {
        public static NextSceneManager Instance;
        public string nextSceneName;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            { 
              Destroy(this.gameObject);
            }
        }



    }
}
