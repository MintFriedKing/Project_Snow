using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PS.Player;

namespace PS
{
    public class PlayerHealth : Health
    {
        private Player player;
        //public PlayerHealthBar playerHealthBar;
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
            player.playerState = PlayerState.DIE; //죽음 상태로 만들어주고 
            GameManager.Instance.DieCharacterSwap();//캐릭이 3개니까  남은캐릭터가 있다면 그쪽으로 바꿔줘야 하고 죽은 캐릭터는 선택 못해야함
                                                    //

        }
        public override void TakeDamage(float _amount, Vector3 _direction)
        {
            currentHealth -= _amount; // 딜 들어온거 체력 수치 깍음 
            UIManager.Instance.UpdateCharacterList();

            if (currentHealth <= 0.0f)
            {
                Die();
            }
        }
    }
}
