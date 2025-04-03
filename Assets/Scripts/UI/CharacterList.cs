using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PS
{
    public class CharacterList : MonoBehaviour
    {
        public int selectNumber;
        public Slider hpSlider;
        public Color dieColor;
        // Start is called before the first frame update
        void Start()
        {
            selectNumber = GameManager.Instance.SelectNumber - 1;
            hpSlider.maxValue = GameManager.Instance.players[selectNumber].player.PlayerHealth.Maxhealth;
            hpSlider.value = GameManager.Instance.players[selectNumber].player.PlayerHealth.CurrentHealth;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
