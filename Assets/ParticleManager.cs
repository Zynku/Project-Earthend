using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public ParticleSystem hitParticles;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayHitParticles(Vector2 playPosition)
    {
        hitParticles.transform.position = playPosition;
        hitParticles.Play(true);
    }
}
