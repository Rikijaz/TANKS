using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Image fillImage;                      
    [SerializeField] Color fullHealthColor = Color.green;
    [SerializeField] Color zeroHealthColor = Color.red;
    [SerializeField] GameObject explosionPrefab;

    public float startingHealth { get; set; }
    public float currentHealth { get; private set; }

    public float cachedHealth;

    public static readonly float initialHealth = 100f;

    private AudioSource explosionAudio;
    private ParticleSystem explosionParticles;
    
    private bool dead;
    
    private void Awake()
    {
        explosionParticles = 
            Instantiate(explosionPrefab).GetComponent<ParticleSystem>();
        explosionAudio = explosionParticles.GetComponent<AudioSource>();

        explosionParticles.gameObject.SetActive(false);

        startingHealth = initialHealth;
    }

    private void OnEnable()
    {
        currentHealth = startingHealth;
        slider.maxValue = currentHealth;

        dead = false;

        SetHealthUI();
    }

    public void SetBossHealthHUD(Slider slider, Image fillImage)
    {
        this.slider = slider;
        this.fillImage = fillImage;
    }

    // Adjust the tank's current health, update the UI based on the new health 
    // and check whether or not the tank is dead.
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        SetHealthUI();

        if (currentHealth <= 0f && !dead)
        {
            OnDeath();
        }
    }

    // Adjust the value and colour of the slider.
    private void SetHealthUI()
    {
        slider.value = currentHealth;
        fillImage.color = Color.Lerp(
            zeroHealthColor, 
            fullHealthColor, 
            currentHealth / startingHealth);
    }

    // Play the effects for the death of the tank and deactivate it.
    private void OnDeath()
    {
        dead = true;

        explosionParticles.transform.position = transform.position;
        explosionParticles.gameObject.SetActive(true);
        explosionParticles.Play();
        explosionAudio.Play();

        gameObject.SetActive(false);
    }
}