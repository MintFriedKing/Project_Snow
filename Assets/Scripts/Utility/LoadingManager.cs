using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PS
{
    public class LoadingManager : MonoBehaviour
    {
        public Image backgroundImage;
        public List<Sprite> backGroundSprites;
        private void Awake()
        {
           
        }
        private void Start()
        {
            SetBackGroundImage();
        }
        private void SetBackGroundImage()
        {
            
            int RandomValue =Random.Range(0, backGroundSprites.Count);
            Debug.Log(RandomValue);
            backgroundImage.sprite = backGroundSprites[RandomValue];
        }


    }
}
