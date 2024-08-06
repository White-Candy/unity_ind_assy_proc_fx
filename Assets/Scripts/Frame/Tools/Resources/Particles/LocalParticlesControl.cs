using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalParticlesControl : MonoBehaviour
{
    [HideInInspector]
    public MeshRenderer meshRenderer;

    [HideInInspector]
    public ParticleSystem Particle;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        Particle = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (meshRenderer?.enabled != false)
        {
            Particle?.Play();
        }
        else
        {
            Particle?.Stop();
        }
    }
}
