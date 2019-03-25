﻿using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float startingHealth = 100f;          
    public Slider slider;                        
    public Image fillImage;                      
    public Color fullHealthColor = Color.green;  
    public Color zeroHealthColor = Color.red;    
    public GameObject explosionPrefab;

    private AudioSource explosionAudio;
    private ParticleSystem explosionParticles;
    private float currentHealth;
    private bool dead;
    
    private void Awake()
    {
        explosionParticles = 
            Instantiate(explosionPrefab).GetComponent<ParticleSystem>();
        explosionAudio = explosionParticles.GetComponent<AudioSource>();

        explosionParticles.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        currentHealth = startingHealth;
        dead = false;

        SetHealthUI();
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