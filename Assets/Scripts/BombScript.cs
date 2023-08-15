using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    public GameManager gameManager;

    public ParticleSystem destructionParticles;

    public void Explode()
    {
        Instantiate(destructionParticles, transform.position, Quaternion.identity);
        gameManager.objectManager.BombDetonate();
        Destroy(gameObject);
    }
}
