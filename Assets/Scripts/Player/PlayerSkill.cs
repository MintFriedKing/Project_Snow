using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class PlayerSkill : MonoBehaviour
    {
        public GameObject skillPrefab;
        public Transform  targetTransform;
        public PlayerAnimationManager playerAnimationManager;
        public virtual void UseSkill()
        {
            //
            GameObject skill = Instantiate(skillPrefab, targetTransform.position,Quaternion.identity);
            skill.transform.SetParent(targetTransform);
            
        }
       
    }
}
