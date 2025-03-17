using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    private Enemy enemy;
    private Health health;

    public Health Health { get { return health; } set { health = value; } }


    
    public void OnRaycastHit(Gun _gun ,Vector3 _direction)
    {
        health.TakeDamage(_gun.Damage,_direction);
    }

   

}
