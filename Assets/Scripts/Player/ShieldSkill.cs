using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PS.Player;

namespace PS
{
    public class ShieldSkill : PlayerSkill
    {
        private GameObject shieldObject;
        public Player player;
        public FullBodyBipedIK fullBodyBipedIK;
        public override void UseSkill()
        {
            StartCoroutine(UseSkillAnimationSycnc());
        }
        public IEnumerator UseSkillAnimationSycnc()
        {
            SkillManager.Instance.IsShield = true;
            //애니메이션을 실행하기전에 총에 고정된 ik를 풀어준다.
            fullBodyBipedIK.solver.leftHandEffector.positionWeight = 0.0f;
            //weaponik로 사격시 조준ik를  구현하고 있으니 스킬 쓸때 잠시 풀어준다.
            PlayerState previousPlayerState = player.playerState;
            if (previousPlayerState == PlayerState.COMBAT)
            {
                player.playerState = Player.PlayerState.IDLE;
            }
            //애니메이션 실행
            playerAnimationManager.Sheild();
            //지정한 애니메이션으로 이동할때까지 대기한다.
            while (!playerAnimationManager.PlayerAnimator.GetCurrentAnimatorStateInfo(1).IsName("ShieldSkill"))
            {        
                yield return null;
            }
            //내가 지정한 애니메이션이 실행이 완료될때까지 대기한다.
            while (playerAnimationManager.PlayerAnimator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f || playerAnimationManager.PlayerAnimator.IsInTransition(1))
            {
                yield return null;
            }
            //ik를 원상복귀한다.
            fullBodyBipedIK.solver.leftHandEffector.positionWeight = 1.0f;
            //애니메이션이 완료되면 쉴드 생성 및 상태처리
            GameObject shield = Instantiate(skillPrefab,targetTransform.position,Quaternion.identity);//ui 이펙트 생성
            shield.transform.SetParent(targetTransform);
            SkillManager.Instance.IsShield = false;      
            UIManager.Instance.ShieldBar.gameObject.SetActive(true);
            UIManager.Instance.ShieldBar.Init();   //UI 처리
            //원래 상태로 복기 
            if (previousPlayerState == PlayerState.COMBAT)
            {
                player.playerState = Player.PlayerState.COMBAT;
            }
            yield return new WaitForSeconds(1f);

        }
    }

}

