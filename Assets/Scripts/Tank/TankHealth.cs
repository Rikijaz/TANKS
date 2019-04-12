using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;                      
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color zeroHealthColor = Color.red;
    [SerializeField] private GameObject explosionPrefab;

    public float StartingHealth { get; set; }
    public float CurrentHealth { get; private set; }
    public float CachedHealth { get; set; }

    public const float InitialHealth = 100f;

    private AudioSource explosionAudio;
    private ParticleSystem explosionParticles;
    
    private bool dead;
    
    private void Awake()
    {
        explosionParticles = 
            Instantiate(explosionPrefab).GetComponent<ParticleSystem>();
        explosionAudio = explosionParticles.GetComponent<AudioSource>();

        explosionParticles.gameObject.SetActive(false);

        StartingHealth = InitialHealth;
    }

    private void OnEnable()
    {
        CurrentHealth = StartingHealth;
        slider.maxValue = CurrentHealth;

        dead = false;

        SetHealthUI();
    }

    public void SetBossHealthHUD(Slider slider, Image fillImage)
    {
        this.slider = slider;
        this.fillImage = fillImage;
    }

    public void Heal(float amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, CurrentHealth, StartingHealth);
        SetHealthUI();
    }

    // Adjust the tank's current health, update the UI based on the new health 
    // and check whether or not the tank is dead.
    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        SetHealthUI();

        if (CurrentHealth <= 0f && !dead)
        {
            OnDeath();
        }
    }

    // Adjust the value and colour of the slider.
    private void SetHealthUI()
    {
        slider.value = CurrentHealth;
        fillImage.color = Color.Lerp(
            zeroHealthColor, 
            fullHealthColor,
            CurrentHealth / StartingHealth);
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