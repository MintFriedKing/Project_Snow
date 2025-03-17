using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class EnemyWeapon : MonoBehaviour
    {
        private float damage;   
        private SphereCollider hitSphereCollider;
        public  SphereCollider HitSphereCollider { get { return hitSphereCollider; } set { hitSphereCollider = value; } }
        public  float Damage { get { return damage; } set { damage = value; } }

      
    }
}
