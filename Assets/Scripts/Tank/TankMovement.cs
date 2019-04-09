﻿using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int playerNumber = 1;
    public float speed = 12f;
    public float turnSpeed = 180f;
    public AudioSource movementAudio;
    public AudioClip engineIdling;
    public AudioClip engineDriving;
    public float pitchRange = 0.2f;

    private string movementAxisName;
    private string turnAxisName;
    private Rigidbody rigidbody;
    private float movementInputValue;
    private float turnInputValue;
    private float originalPitch;
    private ParticleSystem[] particleSystems;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }


    private void OnEnable()
    {
        rigidbody.isKinematic = false;
        movementInputValue = 0f;
        turnInputValue = 0f;

        for (int i = 0; i < particleSystems.Length; ++i)
        {
            particleSystems[i].Play();
        }
    }


    private void OnDisable()
    {
        rigidbody.isKinematic = true;

        for (int i = 0; i < particleSystems.Length; ++i)
        {
            particleSystems[i].Stop();
        }
    }


    private void Start()
    {
        movementAxisName = "Vertical" + playerNumber;
        turnAxisName = "Horizontal" + playerNumber;

        originalPitch = movementAudio.pitch;
    }

    // Store the player's input and make sure the audio for the engine 
    // is playing.
    private void Update()
    {
        movementInputValue = Input.GetAxis(movementAxisName);
        turnInputValue = Input.GetAxis(turnAxisName);

        EngineAudio();
    }


    // Play the correct audio clip based on whether or not the tank is moving 
    // and what audio is currently playing.
    private void EngineAudio()
    {
        if ((Mathf.Abs(movementInputValue) < 0.1f) && 
            (Mathf.Abs(turnInputValue) < 0.1f))
        {
            if (movementAudio.clip == engineDriving)
            {
                movementAudio.clip = engineIdling;
                movementAudio.pitch = Random.Range(
                    originalPitch - pitchRange, 
                    originalPitch + pitchRange);
                movementAudio.Play();
            }
        }
        else
        {
            if (movementAudio.clip == engineIdling)
            {
                movementAudio.clip = engineDriving;
                movementAudio.pitch = Random.Range(
                    originalPitch - pitchRange, 
                    originalPitch + pitchRange);
                movementAudio.Play();
            }
        }
    }

    // Move and turn the tank.
    private void FixedUpdate()
    {
        Move();
        Turn();
    }

    // Adjust the position of the tank based on the player's input.
    private void Move()
    {
        Vector3 movement = transform.forward * movementInputValue * speed * 
            Time.deltaTime;
        rigidbody.MovePosition(rigidbody.position + movement);
    }

    // Adjust the rotation of the tank based on the player's input.
    private void Turn()
    {
        float turn = turnInputValue * turnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rigidbody.MoveRotation(rigidbody.rotation * turnRotation);
    }
}