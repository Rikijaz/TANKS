using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    [SerializeField] LayerMask tankMask;
    [SerializeField] ParticleSystem explosionParticles;
    [SerializeField] AudioSource explosionAudio;          
    
    private float maxDamage = 100f;
    private float explosionForce = 1000f;
    private float maxLifeTime = 2f;
    private float explosionRadius = 5f;              

    private void Start()
    {
        Destroy(gameObject, maxLifeTime);
    }

    // Find all the tanks in an area around the shell and damage them.
    private void OnTriggerEnter(Collider other)
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            explosionRadius,
            tankMask);

        for (int i = 0; i < colliders.Length; ++i)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            if (targetRigidbody)
            {
                targetRigidbody.isKinematic = false;

                targetRigidbody.AddExplosionForce(
                    explosionForce, 
                    transform.position, 
                    explosionRadius);

                TankHealth targetHealth = 
                    targetRigidbody.GetComponent<TankHealth>();

                if (targetHealth)
                {
                    float damage = CalculateDamage(targetRigidbody.position);
                    targetHealth.TakeDamage(damage);
                }
            }
        }

        explosionParticles.transform.parent = null;
        explosionParticles.Play();
        explosionAudio.Play();

        Destroy(
            explosionParticles.gameObject,
            explosionParticles.main.duration);
            Destroy(gameObject);
    }

    // Calculate the amount of damage a target should take based on it's position.
    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionToTarget = targetPosition - transform.position;
        float explosionDistance = explosionToTarget.magnitude;
        float relativeDistance = (explosionRadius - explosionDistance) / explosionRadius;
        float damage = relativeDistance * maxDamage;

        damage = Mathf.Max(0f, damage);

        return damage;
    }
}