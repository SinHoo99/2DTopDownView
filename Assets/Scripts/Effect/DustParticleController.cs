using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DustParticleController : MonoBehaviour
{
    [SerializeField] private bool createDustWalk = true;
    [SerializeField] private ParticleSystem dustParticleSystem;

    public void CreateDustParticles()
    {
        dustParticleSystem.Stop();
        dustParticleSystem.Play();
    }
}
