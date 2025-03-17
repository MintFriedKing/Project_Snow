using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using PS;

namespace PS
{
    [System.Serializable]
    public class VFXAsset
    {
        public string VFXName;
        public GameObject VFXPrefab;
        [Tooltip("If a fire point is not provided it will spawn in the VFXMarker position.")]
        public Transform VFXFirePoint;
        [Tooltip("Used in case we want to parent the VFXPrefab.")]
        public Transform VFXParent;
        [Tooltip("Offsets the VFXPrefab a certain amount.")]
        public Vector3 VFXOffset;
        [Space]
        [Tooltip("When ON it will spawn if you hold the right mouse button")]
        public bool spawnWithRightMouseButton = false;
        [Tooltip("Should the VFXPrefab be spawned alternatively between two fire points? Only works if there's an alternateFirePoint")]
        public bool alternateBetweenFirePoints = false;
        [Tooltip("Only use alternate fire point to spawn left hand right hand for example")]
        public Transform alternateFirePoint;
        [Space]
        public List<AudioClip> SFX;
        [Tooltip("Mostly used with a projectile to rotate to a direction and add velocity. When used in a AoE it will only rotate towards where we are aiming.")]
        public bool rotateToDestination = false;
        [Tooltip("Destroy the instantiated vfx after how many seconds?")]
        public float delayToDestroy = 10;
    }

    [System.Serializable]
    public class VFXParameters
    {
        public string AbilityName;
        [Tooltip("Used to select which animation to play in the Animator.")]
        public AttackAnimation attackAnimation = AttackAnimation.Attack01;
        [Tooltip("Ability types. AOE doesnt need firepoint or projectile speed")]
        public AbilityType abilityType;
        [Tooltip("Cooldown before shooting this ability again")]
        public float cooldown = 2;
        [Tooltip("How much time is the character immovable to cast the spell?")]
        public float freezeTime;
        [Tooltip("The travelling speed of the projecitle. Only for Projectiles.")]
        public float projectileSpeed;
        [Tooltip("Rotates only in the Y axis. Excludes X and Z rotations.")]
        public bool rotateOnlyY = true;
        [Tooltip("The VFX Asset is to decalre the prefab, firepoint, parent and offset")]
        public List<VFXAsset> VFXAsset;
        [Space]
        [Tooltip("Should the ground marker be Free or Locked? Where locked is at the foot of the character and free is within a certain radius.")]
        public MarkerType markerType;
        [Tooltip("The Ground Marker Prefab")]
        public GameObject VFXMarker;
        [Tooltip("The Ground Marker position. Only for Projectiles")]
        public Transform VFXMarkerPosition;
        [Header("POST-PRODUCTION")]
        [Tooltip("Camera Shake?")]
        public bool shake;
        [Tooltip("After how many seconds to shake? And how many times?")]
        public List<float> delays;
        public List<float> durations;
        public List<float> amplitudes;
        public List<float> frequencies;
        [Tooltip("Chromatic Aberration Punch Effect?")]
        public bool chromaticAberration;
        [Tooltip("Goes from 0 to 1. 0 is no chromatic aberration")]
        public List<float> chromaticGoal;
        [Tooltip("Chromatic Rate is same as Refresh Rate. 0.05 is a good rate")]
        public List<float> chromaticRate;
    }

    public class BossVFXSpawnAbilities : MonoBehaviour
    {   
        public AudioSource audioSource;
        [Tooltip("Dectect collisions in wich layer?")]
        public LayerMask collidingLayer;
        [FormerlySerializedAs("VFX")]
        [Tooltip("This is where we assign prefabs. List of effects will be automatically assigned to 1, 2, 3 and 4 keys respectively.")]
        public List<VFXParameters> Abilities;
        [Space]
        [Header("POST-PRODUCTION")]
        [Tooltip("Assign a Global Volume to use Post Processing effects like chromatic aberration")]
        public Volume volume;
        [Tooltip("Assign an impulse source for camera shake. Impulse Source should be attached to the camera")]
        public CinemachineImpulseSource impulseSource;
        public VFXParameters effectToSpawn;
        private int currentAttack = 0;
        private bool aiming = false;
        public bool attacking = false;
        private bool chromaticIncrease = false;
        private bool leftHand;
        private bool spawnedRMBEffects;
        public GameObject vfxMarker;
        private Vector3 destination;
        private List<GameObject> RMBEffects = new List<GameObject>();
        private List<VisualEffect> RMBVFXGraphs = new List<VisualEffect>();
        public BossAnimationManger bossAnimationManger;
        public Boss boss;

        private void Awake()
        {
            boss = this.GetComponent<Boss>();
        }

        public void VFXSelecter(int number)
        {
            currentAttack = number;
            if (Abilities.Count > number - 1)
            {
                effectToSpawn = Abilities[number - 1];
                if (effectToSpawn.VFXMarker != null)
                {
                    if (vfxMarker == null)
                    {
                        vfxMarker = new GameObject();
                        vfxMarker.name = "VfxMarker";
                        vfxMarker.SetActive(false);
                    }
                    if (vfxMarker.name != effectToSpawn.VFXMarker.name)
                    {
                        if (effectToSpawn.VFXMarker != null)
                        {
                            Destroy(vfxMarker);
                            vfxMarker = Instantiate(effectToSpawn.VFXMarker) as GameObject;//vfxMarker = effectToSpawn.VFXMarker;
                            vfxMarker.name = effectToSpawn.VFXMarker.name;
                            vfxMarker.SetActive(false);
                        }
                    }
                }
                else
                {
                    Destroy(vfxMarker);
                    vfxMarker = null;
                }
            }
            else
                Debug.Log("Please assign a VFX in the inspector.");
        }   

        public IEnumerator ThrrowHammer()
        {
            attacking = true;
            for (int i = 0; i < effectToSpawn.VFXAsset.Count; i++)
            {
                if (effectToSpawn.delays[i] > 0)
                {
                    yield return new WaitForSeconds(effectToSpawn.delays[i]); //설정된 시간만큼 딜레이 
                }
                if (!effectToSpawn.VFXAsset[i].spawnWithRightMouseButton)
                {
                    GameObject projectileVFX;

                    if (effectToSpawn.VFXAsset[i].alternateBetweenFirePoints)
                    {
                        if (leftHand)
                        {
                                            
                            projectileVFX = Instantiate(effectToSpawn.VFXAsset[i].VFXPrefab, effectToSpawn.VFXAsset[i].alternateFirePoint.position + effectToSpawn.VFXAsset[i].VFXOffset, Quaternion.identity) as GameObject;
                        }
                        else
                        {       
                    
                            projectileVFX = Instantiate(effectToSpawn.VFXAsset[i].VFXPrefab, effectToSpawn.VFXAsset[i].VFXFirePoint.position + effectToSpawn.VFXAsset[i].VFXOffset, Quaternion.identity) as GameObject;
                            
                        }
                    }
                    else 
                    {
                        projectileVFX = Instantiate(effectToSpawn.VFXAsset[i].VFXPrefab, effectToSpawn.VFXAsset[i].VFXFirePoint.position + effectToSpawn.VFXAsset[i].VFXOffset, Quaternion.identity) as GameObject;
                        if (effectToSpawn.VFXAsset[i].VFXParent != null)
                        {
                            projectileVFX.transform.SetParent(effectToSpawn.VFXAsset[i].VFXParent);
                            projectileVFX.transform.localPosition = Vector3.zero;
                            projectileVFX.transform.localEulerAngles = Vector3.zero;
                        }
                    }
                    if (effectToSpawn.VFXAsset[i].rotateToDestination)
                    {
                        if (vfxMarker != null)
                        {
                            Ray newRay = new Ray(vfxMarker.transform.position, vfxMarker.transform.forward);
                            RotateToDestination(projectileVFX, newRay.GetPoint(100), effectToSpawn.rotateOnlyY);
                        }
                        else
                        {
                            RotateToDestination(projectileVFX, destination, effectToSpawn.rotateOnlyY);
                        }
                        var rb = projectileVFX.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                         
                            if (effectToSpawn.VFXMarker != null)
                            {
                                Player player = Player.Instance;
                                Vector3 direction = (player.transform.position - effectToSpawn.VFXAsset[i].VFXFirePoint.position).normalized;
                                if (player != null)
                                {
                                    projectileVFX.GetComponent<Rigidbody>().velocity = direction * effectToSpawn.projectileSpeed;
                                
                                }
                                 
                            }
                            else
                            {
                             
                                projectileVFX.GetComponent<Rigidbody>().velocity = transform.forward * effectToSpawn.projectileSpeed;
                            }
                        }
                        else
                        {
                            Debug.Log("This projectile doesn't have a rigidbody.");
                        }

                        Destroy(projectileVFX, effectToSpawn.VFXAsset[i].delayToDestroy);
                    }
                    else
                    {
                        Destroy(projectileVFX, effectToSpawn.VFXAsset[i].delayToDestroy);
                    }
                }
            }
            yield return new WaitForSeconds(effectToSpawn.cooldown);
            attacking = false;
            for (int i = 0; i < effectToSpawn.VFXAsset.Count; i++)
            {
                if (effectToSpawn.VFXAsset[i].alternateBetweenFirePoints)
                {
                    if (leftHand)

                        leftHand = false;
                    else
                        leftHand = true;

                    break;
                }
            }

        }
        public IEnumerator ThunderAOE()
        {
            attacking = true;

            for (int i = 0; i < effectToSpawn.VFXAsset.Count; i++)
            {
                if (!effectToSpawn.VFXAsset[i].spawnWithRightMouseButton)
                {
                    GameObject aoeVFX;

                    yield return new WaitForSeconds(effectToSpawn.delays[i]);  //애니메이션 타이밍 맞출려는 듯? 

                    if (effectToSpawn.VFXAsset[i].VFXFirePoint != null)
                    {
                        Player player = Player.Instance;
                        //----- 생성
                        if (effectToSpawn.VFXAsset[i].VFXFirePoint != null)
                        {
                            // 광역기 특수 처리
                            if (effectToSpawn.VFXAsset[i].VFXName == "Thunder Storm" && currentAttack == 4)
                            {
                                aoeVFX = Instantiate(effectToSpawn.VFXAsset[i].VFXPrefab, player.transform.position, Quaternion.identity) as GameObject;
                            }
                            else
                            {
                                aoeVFX = Instantiate(effectToSpawn.VFXAsset[i].VFXPrefab, effectToSpawn.VFXAsset[i].VFXFirePoint.position + effectToSpawn.VFXAsset[i].VFXOffset, Quaternion.identity) as GameObject;
                            }                        
                        }
                        else
                        {
                            aoeVFX = Instantiate(effectToSpawn.VFXAsset[i].VFXPrefab, vfxMarker.transform.position + effectToSpawn.VFXAsset[i].VFXOffset, Quaternion.identity) as GameObject;
                        }

                        if (currentAttack == 2 && effectToSpawn.VFXAsset[i].VFXName == "Earthshatter")
                        {
                            boss.TriAngleSkill.enabled = true;   
                        }
                       
                        if (effectToSpawn.VFXAsset[i].VFXParent != null)
                        {
                            aoeVFX.transform.SetParent(effectToSpawn.VFXAsset[i].VFXParent);
                            aoeVFX.transform.localPosition = Vector3.zero;
                            aoeVFX.transform.localEulerAngles = Vector3.zero;
                        }

                        //로테이션 
                        if (effectToSpawn.VFXAsset[i].rotateToDestination == true)
                        {
                            if (effectToSpawn.VFXAsset[i].VFXName == "Hammer Punch")
                            {
                                //Ray newRay = new Ray(vfxMarker.transform.position, player.transform.position);
                                //Vector3 direction = (player.transform.position - this.gameObject.transform.position).normalized;
                                //Ray newRay = new Ray(vfxMarker.transform.position, player.transform.position);
                                Ray newRay = new Ray(vfxMarker.transform.position, vfxMarker.transform.forward);
                                RotateToDestination(aoeVFX, newRay.GetPoint(100), effectToSpawn.rotateOnlyY);

                            }
                            else 
                            {
                                Ray newRay = new Ray(vfxMarker.transform.position, vfxMarker.transform.forward);
                                RotateToDestination(aoeVFX, newRay.GetPoint(100), effectToSpawn.rotateOnlyY);
                            }
                        }

 
                        var trails = aoeVFX.GetComponent<DetachGameObjects>();
                        if (trails != null)
                        {
                            StartCoroutine(trails.Detach(effectToSpawn.VFXAsset[i].delayToDestroy - 0.1f));
                        }

                        Destroy(aoeVFX, effectToSpawn.VFXAsset[i].delayToDestroy);
                    }
                }
            }
            yield return new WaitForSeconds(effectToSpawn.cooldown); //
          
            attacking = false;

            //for (int i = 0; i < effectToSpawn.VFXAsset.Count; i++)
            //{
            //    if (effectToSpawn.VFXAsset[i].alternateBetweenFirePoints)
            //    {
            //        if (leftHand)
            //            leftHand = false;
            //        else
            //            leftHand = true;

            //        break;
            //    }
            //}
        }
        void RotateToDestination(GameObject obj, Vector3 destination, bool onlyY)
        {
            if (currentAttack == 3) //Hammer Punch
            {
                var direction = destination - obj.transform.position;
                var rotation = Quaternion.LookRotation(direction);
                if (onlyY)
                {
                    rotation.x = 0;
                    rotation.z = 0;
                }
                obj.transform.rotation = rotation;
            }
            else
            {
                var direction = destination - obj.transform.position;
                var rotation = Quaternion.LookRotation(direction);

                if (onlyY)
                {
                    rotation.x = 0;
                    rotation.z = 0;
                }
                obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, rotation, 1);
            }
        }

        IEnumerator ShakeCameraWithImpulse(List<float> delays, List<float> shakeDuration, List<float> shakeAmplitude, List<float> shakeFrequency)
        {
            for (int i = 0; i < delays.Count; i++)
            {
                yield return new WaitForSeconds(delays[i]);
                impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = shakeDuration[i];
                impulseSource.m_ImpulseDefinition.m_AmplitudeGain = shakeAmplitude[i];
                impulseSource.m_ImpulseDefinition.m_FrequencyGain = shakeFrequency[i];
                impulseSource.GenerateImpulse();
            }
        }
    }
}
