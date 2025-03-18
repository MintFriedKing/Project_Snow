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
        public static SkillManager Instance;
        
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
        private void Update()
        {
            
        }

    }
}
