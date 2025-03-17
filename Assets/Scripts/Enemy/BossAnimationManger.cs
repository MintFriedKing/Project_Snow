using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class BossAnimationManger : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;
        public void Movement(float _inputValue)
        {
            animator.SetFloat("MoveInput",_inputValue);
        }
        public void ThrrowHammer()
        {
            animator.SetTrigger("Attack01");
        }
        public void ThunderAOE(int _attackNumber)
        {
            animator.SetTrigger("Attack0"+ _attackNumber.ToString());
            Debug.Log("Attack0" + _attackNumber.ToString());
            //animator.SetTrigger("Attack4");

        }
        public void Jump()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !animator.IsInTransition(0))
            {
                animator.SetTrigger("Jump");
            }
        }
    }
}
