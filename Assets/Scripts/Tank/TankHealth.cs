using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float startingHealth = 100f;          
    public Slider slider;                        
    public Image fillImage;                      
    public Color fullHealthColor = Color.green;  
    public Color zeroHealthColor = Color.red;    
    public GameObject explosionPrefab;
    
    /*
    private AudioSource explosionAudio;          
    private ParticleSystem explosionParticles;   
    private float currentHealth;  
    private bool m_Dead;            


    private void Awake()
    {
        explosionParticles = Instantiate(explosionPrefab).GetComponent<ParticleSystem>();
        explosionAudio = explosionParticles.GetComponent<AudioSource>();

        explosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        currentHealth = startingHealth;
        m_Dead = false;

        SetHealthUI();
    }
    */

    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
    }
}