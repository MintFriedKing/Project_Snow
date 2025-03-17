using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace PS
{
    public class LaserBeamSkill : PlayerSkill
    {   
        public float damage;
        public Player player;
        public PlayerShootManager playerShootManager;
        public Gun gun;
        public GameObject laserBeam;
        public Rigidbody rigidbody;
        public float recoilForce;
        public LayerMask excludeLayerMask;
        public override void UseSkill()
        {
            if (SkillManager.Instance.IsLaserCahrge == false)
            {
                StartCoroutine(FireLaserRoutine());
            }
            //----------------------------------------------------------------------------------------
            //Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));  //화면 정중앙
            //Vector3 lookDirection = ray.direction;  //그쪽 방향
            //Vector3 direction = lookDirection;
            //RotateToDestination(laser,direction,true);
        }

        private IEnumerator FireLaserRoutine()
        {
            //차지중임을 알림
            SkillManager.Instance.IsLaserCahrge = true;
         
            // -- 스킬 사용에 맞는 상태로 전환 
            player.SetPlayerState(Player.PlayerState.COMBAT);
            // ---  차지 이펙트  로테이션 조정 
            this.transform.rotation = Quaternion.LookRotation(playerShootManager.AimPosition);

            //차지 이펙트 생성
            GameObject ChargeVFX = Instantiate(skillPrefab); 
            ChargeVFX.transform.SetParent(gun.fireTramsform);
            ChargeVFX.transform.localPosition = Vector3.zero;
            ChargeVFX.transform.rotation = gun.fireTramsform.rotation;
            Destroy(ChargeVFX.transform.gameObject, 1f); 
            yield return new WaitForSeconds(1.1f);  //차지까지 대기 
  
            GameObject laser = Instantiate(laserBeam);
            Vector3 laserDirection = (playerShootManager.AimPosition - gun.fireTramsform.position).normalized;
            laser.transform.SetParent(gun.fireTramsform);
            laser.transform.localPosition = Vector3.zero;
            laser.transform.rotation = Quaternion.LookRotation(laserDirection);
            TakeDamage(laser.transform.position, laserDirection);
            Destroy(laser.transform.gameObject, 0.3f);

            yield return new WaitForSeconds(0.5f);
            playerAnimationManager.LaserBeam();       
            Vector3 recoilDirection = -transform.forward;
            rigidbody.AddForce(recoilDirection * recoilForce, ForceMode.Impulse);
            SkillManager.Instance.IsLaserCahrge = false;
            bool waitBool = SkillManager.Instance.IsLaserCahrge;
            Debug.Log("SkillManager.IsLaserCahrge: " + SkillManager.Instance.IsLaserCahrge);
            yield return new WaitUntil(() => waitBool);
            Debug.Log("WaitUntil 종료 후");
        }

        private void TakeDamage(Vector3 _startPoint, Vector3 _direction)
        {
            
            RaycastHit[] hits = Physics.RaycastAll(_startPoint, _direction, 1000,excludeLayerMask);

            if (hits.Length == 0)
            {
                Debug.Log("충돌한 오브젝트가 없습니다.");
            }

            foreach (RaycastHit hit in hits)
            {
                GameObject Enemy = hit.collider.gameObject;
                if (Enemy.CompareTag("Enemy"))
                {
                    Enemy.GetComponent<Health>().TakeDamage(damage, Vector3.zero);
                }
                Debug.Log(Enemy.name);


            }
            Debug.Log("");



        }

    }
  
}
