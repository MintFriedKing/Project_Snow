using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class SkillManager : MonoBehaviour
    {

        [SerializeField,Header("Shield Value")]
        private bool isShield;
        [SerializeField, Header("LaserCharge")]
        private bool isLaserCahrge;
        [SerializeField, Header("Healing")]
        private bool isHealing;
        [SerializeField]
        private float[] skillCoolTimes = new float[3];
        public static SkillManager Instance;
        public float currnetCircleSliderValue;
        public float[] SkillCoolTimes { get { return skillCoolTimes; } set { skillCoolTimes = value; } }
        public bool IsShield { get { return isShield; } set { isShield = value;  } }
        public bool IsLaserCahrge { get { return isLaserCahrge; } set { isLaserCahrge = value; } }
        public bool IsHealing { get { return isHealing; } set { isHealing = value; } }
        private void Awake()
        {      
            isShield = false;
            isLaserCahrge = false;
            isHealing = false;
            Instance = this;
        }
        public IEnumerator CoolDownRoutine(int _number)
        {
            float currentPlayerCoolTime;
            while (skillCoolTimes[_number] > 0)
            {
                skillCoolTimes[_number] -= Time.deltaTime;
                currentPlayerCoolTime = Mathf.CeilToInt(skillCoolTimes[_number]);
                float normalizeTime = (skillCoolTimes[_number] - 0) / (GameManager.Instance.players[_number].skillCoolTime - 0);
                if (UIManager.Instance.nomalSkilltext.gameObject.activeSelf == true)
                {                   
                    UIManager.Instance.nomalSkilltext.text = currentPlayerCoolTime.ToString();
                }
                if (GameManager.Instance.players[_number].player.gameObject.activeSelf ==true
                    && GameManager.Instance.players[_number].player.IsUseSkill ==true)
                {
                    foreach (CircleSlider circleSlider in UIManager.Instance.circleSliders)
                    {
                        circleSlider.UpdateSlider(normalizeTime);
                    }
                }
                yield return null;
            }      
            GameManager.Instance.players[_number].player.IsUseSkill = false;
            skillCoolTimes[_number] = GameManager.Instance.players[_number].skillCoolTime;
        }

      
    }
}
