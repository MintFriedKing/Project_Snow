using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class JumpToPlayer : BehaviorDesigner.Runtime.Tasks.Action
    {
      
        public Boss boss;
        #region 물리점프 기반 변수
        public float jumpForce =10f;
        public float forwardForce =5f;
        public Rigidbody rigidBody;
        #endregion
        public int jumpCount = 0;
        [Header("점프 높이 sin 곡선 진폭")]
        public float jumpAmplitude = 3f;
        [Header("점프 속도 sin 곡선 주파수")]
        public float jumpFrequency = 2f;
        [Header("앞으로 이동하는 속도")]
        public float forwardSpeed = 3f;
        private float startY; //처음 Y(높이) 저장
        private float timeElapsed = 0f; // 점프 시간 트래킹 
        private List<Vector3> trajectoryPoints = new List<Vector3>(); //포물선 궤적 저장용 

        private Player player;
        private float startTime;
        private Vector3 direction;
        private bool isJumping =false;
        public float minDistance;
        public float maxDistance;
        public override void OnStart()
        {      
            jumpCount = 0;
            if (boss.navMeshAgent.isOnNavMesh == true)
            {
                boss.navMeshAgent.enabled = false;        
            }    
            if(boss.CurrentDistance > minDistance && boss.CurrentDistance <= boss.MaxDistance )
            {
                boss.StartJump();      
            }     
            boss.RotateToDestination(boss.gameObject,Player.Instance.transform.position,true);
        }
        public override TaskStatus OnUpdate()
        {
            if (boss.isJumping == false && boss.IsGround == true )
            {
                return TaskStatus.Success;
            }
            boss.MoveInSineWave(jumpFrequency, jumpAmplitude , forwardSpeed);
            return TaskStatus.Running;
        }
        //물리 기반 점프 
        private void RigidBodyJump()
        {
            Player player = Player.Instance;
            //1.일단 전방 방향 계산
            Vector3 direction = (player.transform.position - boss.transform.position).normalized;
            direction.y = 0;
            //2.최종 방향 설정 
            Vector3 jumpVelocity = direction * forwardForce + Vector3.up * jumpForce;

            //3. 리지드 바디 적용 
            rigidBody.velocity = jumpVelocity;
        }
     
     

    }
}
