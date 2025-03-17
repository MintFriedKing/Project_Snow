using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PS.Player;

namespace PS
{
    public class PlayerHealth : Health
    {
        private Player player;
        public PlayerHealthBar playerHealthBar;
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            currentHealth = maxhealth;
            player = this.GetComponent<Player>();

        }


        public override void Die()
        {
            player.playerState = PlayerState.DIE;

        }
        public override void TakeDamage(float _amount, Vector3 _direction)
        {
            currentHealth -= _amount; // 딜 들어온거 체력 수치 깍음 
            playerHealthBar.SetHealthBar(currentHealth / maxhealth); //ui처리
            if (currentHealth <= 0.0f)
            {
                Die();
            }
        }
    }
}
