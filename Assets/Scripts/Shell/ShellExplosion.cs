using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask tankMask;
    public ParticleSystem explosionParticles;       
    public AudioSource explosionAudio;              
    public float maxDamage = 100f;                  
    public float explosionForce = 1000f;            
    public float maxLifeTime = 2f;                  
    public float explosionRadius = 5f;              


    private void Start()
    {
        Destroy(gameObject, maxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        return 0f;
    }
}