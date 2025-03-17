using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class ParticleSpeedController : MonoBehaviour
    {
        private ParticleSystem[] particleSystems;
        public float particleSpeed;
        private void Awake()
        {
            particleSystems = this.GetComponentsInChildren<ParticleSystem>();                    
        }
        private void OnEnable()
        {
            SetSimulationSpeed(particleSpeed);
        }
        public void SetSimulationSpeed(float _particleSpeed)
        {
            foreach (var particle in particleSystems)
            {
                var main = particle.main;
                main.simulationSpeed = _particleSpeed;
            }
        }
    }
}
