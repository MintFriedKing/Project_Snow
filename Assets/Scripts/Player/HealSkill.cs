using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PS.Player;

namespace PS
{
    public class HealSkill : PlayerSkill
    {
        public float healTime;
        public Player player;
        public FullBodyBipedIK fullBodyBipedIK;
        private ParticleSystem[] particleSystems;
        
        public void Awake()
        {
            particleSystems = skillPrefab.GetComponentsInChildren<ParticleSystem>();
            foreach (var particle in particleSystems)
            {
                var main = particle.main;
                main.duration = healTime;
            }
        }
        public override void UseSkill()
        {
            StartCoroutine(UseSkillAnimationSycnc());
        }
        public IEnumerator UseSkillAnimationSycnc()
        {
            //애니메이션을 실행하기전에 총에 고정된 ik를 풀어준다.
            fullBodyBipedIK.solver.leftHandEffector.positionWeight = 0.0f;
            PlayerState previousPlayerState = player.playerState;
            if (previousPlayerState == PlayerState.COMBAT)
            {
                player.playerState = Player.PlayerState.IDLE;
            }

            playerAnimationManager.Heal();

            while (playerAnimationManager.PlayerAnimator.GetCurrentAnimatorStateInfo(1).IsName("HealSkill") == false)
            {
                yield return null;
            }
            while (playerAnimationManager.PlayerAnimator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f)
            {
                yield return null;
            }

            fullBodyBipedIK.solver.leftHandEffector.positionWeight = 1.0f;

            GameObject healArea = Instantiate(skillPrefab);
            healArea.transform.position = this.transform.position;
            Destroy(healArea, healTime);
            if (previousPlayerState == PlayerState.COMBAT)
            {
                player.playerState = Player.PlayerState.COMBAT;
            }

        }

    }
}

