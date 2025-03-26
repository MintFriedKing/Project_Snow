using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;
using PS;
using RootMotion.FinalIK;
using static PS.Player;
public class Gun : MonoBehaviour
{
    //public Camera aimCamera;
    [Header("실시간 탄창안에 총알갯수"), SerializeField]
    private int currentAmmo;
    [Header("탄창 하나에 들어갈 수 있는 총알 최대 갯수"), SerializeField]
    private int maxCapacity =30;
    [SerializeField,Header("데미지")]
    private float damage = 15f;
    [SerializeField, Header("사거리")]
    private float range = 100f;
    [SerializeField, Header("총알 발사 간격")]//분당 몇발 이런개념
    private float fireRate =0.12f;
    private float lastFireTime = 0f; // 마지막으로 총알발사한 시점? 
    private float currentBulletSpread;//탄에 퍼짐 정도 -> 계획에는 없지만 총별 탄퍼짐 정도를 줄려면 필요한듯?


    [Header("조준 위치")]
    public Transform aimTransform;
    [Header("총구 위치")]
    public Transform fireTramsform;
    //[Header("총알 궤적")]
    //public LineRenderer bulletLineRenderer;
    [Header("총구 화염")]
    public ParticleSystem[] muzlePaticleSystems;
    [Header("히트 이팩트")]
    public ParticleSystem hitEffectPacticleSystem;
    public LayerMask excludeTarget;
  
    public int MaxCapacity { get { return maxCapacity; } }
    public float Damage { get {return damage; } }
    public  int CurrentAmmo { get { return currentAmmo; } }
    public enum GunState
    { 
        READY, //사격가능
        EMPTY,// 탄창이빔
        RELOADING
    }
    public GunState gunState;
    public float Range { get { return range; } }
    public bool isReLoading;

    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        //bulletLineRenderer = this.GetComponent<LineRenderer>();
        //bulletLineRenderer.positionCount = 2;
        //bulletLineRenderer.enabled = false;

        currentBulletSpread = 0f;
        currentAmmo = maxCapacity;
        gunState = GunState.READY;
        lastFireTime = 0f;
        isReLoading = false;
    }
    public bool Fire(Vector3 _aimTarget)
    {
        //총이 준비상태이면서, 마지막으로 발사한 시점보다 시간이 fireRate 만큼 흘르는것이 발사 조건 
        if (gunState == GunState.READY && Time.time > lastFireTime + fireRate)
        {
            //가우스 분포 나중에 이해하면 대입해보기
            //var xSpread = RandomUtility.GetRandomNormalDistribution()
            Vector3 fireDirection = _aimTarget - fireTramsform.transform.position;
            lastFireTime = Time.time;
            Shoot(fireTramsform.position, fireDirection);
            return true;
        }
        else
        {
            return false;
        }
    }
    //private IEnumerator ShotEffect(Vector3 hitPosition)
    //{
    //    //사격 파티클 재생
    //    Instantiate(weponFlashFX, fireTramsform);
    //    //총알 궤적 그리기
    //    bulletLineRenderer.SetPosition(0,fireTramsform.position); // 시작점은 총구이다.
    //    bulletLineRenderer.SetPosition(1, hitPosition);//선에 끝은 레이 충돌한 위치다.
    //    bulletLineRenderer.enabled =true;

    //    yield return new WaitForSeconds(0.03f); // 궤적이 남는 효과를 줘야 하니까 
    //    bulletLineRenderer.enabled = false;
    //}
    private void Shoot(Vector3 _startPoint,Vector3 _direction)
    {
             
        RaycastHit hit;
        Vector3 hitPosition = Vector3.zero;

        if (Physics.Raycast(_startPoint, _direction, out hit, range, ~excludeTarget))
        {

            //적 데미지입는 부분 
            if (hit.collider.GetComponent<HitBox>() != null)
            {
                var hitBox = hit.collider.GetComponent<HitBox>();
                hitBox.OnRaycastHit(this, _direction);
        
            }
        
            //레이가 충돌했을 경우 
            hitPosition = hit.point;
            hitEffectPacticleSystem.transform.position = hit.point; // 히트 이펙트 위치 지정
            hitEffectPacticleSystem.transform.forward = hit.normal; // forward 지정
            hitEffectPacticleSystem.Emit(1);

        }
        else // Raycast 사거리 밖 
        {
            //충돌한게 없는경우는 최대 사거리를 히트 포인트로 만듬
            hitPosition = _startPoint + _direction * range; // 
        }
        //이펙트 효과 
        //StartCoroutine(ShotEffect(hitPosition));
        foreach (var paritcle in muzlePaticleSystems)
        {
            paritcle.Emit(1);
        }
       
        //자원 처리
        currentAmmo -= 1;
        if (currentAmmo <= 0)
        {
            //총알없음
            gunState = GunState.EMPTY;
        }

    }
    public bool Reload()
    {
        if (gunState == GunState.RELOADING)
        {
            return false;
        }

        StartCoroutine(ReloadRoutine());

        return true;
    }
    private IEnumerator ReloadRoutine()
    {
        isReLoading = true;
        PlayerState playerState = GameManager.Instance.CurrentPlayer.playerState;
        if (playerState == PlayerState.COMBAT)
        {
            GameManager.Instance.CurrentPlayer.playerState = PlayerState.IDLE;
        }
        //재장전 상태로 전환
        gunState = GunState.RELOADING;
        //ik풀고
        if (GameManager.Instance.CurrentPlayer.fullBodyBipedIK != null)
        {
            GameManager.Instance.CurrentPlayer.fullBodyBipedIK.solver.leftHandEffector.positionWeight = 0.0f;     
        }
       //애니메이션 실행
        GameManager.Instance.CurrentPlayer.PlayerAnimationManager.ReLoad();
       
       //실행된 애니메이션으로 전환될때까지 대기
       while (GameManager.Instance.CurrentPlayer.PlayerAnimationManager.PlayerAnimator.GetCurrentAnimatorStateInfo(1).IsName("ReLoad") == false)
       {
            yield return null;
        }
        float reloadTime = GameManager.Instance.CurrentPlayer.PlayerAnimationManager.PlayerAnimator.GetCurrentAnimatorStateInfo(1).length;
        //실행한 애니메이션이 끝날때까지 대기 
        while (GameManager.Instance.CurrentPlayer.PlayerAnimationManager.PlayerAnimator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f)
        {
            yield return null;
        }
        GameManager.Instance.CurrentPlayer.fullBodyBipedIK.solver.leftHandEffector.positionWeight = 1.0f;
        //탄창 계산하고
        var ammoToFill = maxCapacity - currentAmmo;
        //넣어주기
        currentAmmo += ammoToFill;
        //리로드 할때까지 대기한다.
        gunState = GunState.READY;
        if (playerState == PlayerState.COMBAT)
        {
            GameManager.Instance.CurrentPlayer.playerState = PlayerState.COMBAT;
        }
        isReLoading = false;
        yield return new WaitForSeconds(reloadTime);
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Debug.DrawLine(fireTramsform.position,transform.position + transform.forward * 50);
    //}
}
