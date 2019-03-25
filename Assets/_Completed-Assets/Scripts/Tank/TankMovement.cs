using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    public class TankMovement : MonoBehaviour
    {
        public int playerNumber = 1;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.
        public float speed = 12f;                 // How fast the tank moves forward and back.
        public float turnSpeed = 180f;            // How fast the tank turns in degrees per second.
        public AudioSource movementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
        public AudioClip engineIdling;            // Audio to play when the tank isn't moving.
        public AudioClip engineDriving;           // Audio to play when the tank is moving.
		public float pitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.

        private string movementAxisName;          // The name of the input axis for moving forward and back.
        private string turnAxisName;              // The name of the input axis for turning.
        private Rigidbody rigidbody;              // Reference used to move the tank.
        private float movementInputValue;         // The current value of the movement input.
        private float turnInputValue;             // The current value of the turn input.
        private float originalPitch;              // The pitch of the audio source at the start of the scene.
        private ParticleSystem[] particleSystems; // References to all the particles systems used by the Tanks

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            // When the tank is turned on, make sure it's not kinematic.
            rigidbody.isKinematic = false;
           
            // Also reset the input values.
            movementInputValue = 0f;
            turnInputValue = 0f;

            // We grab all the Particle systems child of that Tank to be able to Stop/Play them on Deactivate/Activate
            // It is needed because we move the Tank when spawning it, and if the Particle System is playing while we do that
            // it "think" it move from (0,0,0) to the spawn point, creating a huge trail of smoke
            particleSystems = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particleSystems.Length; ++i)
            {
                particleSystems[i].Play();
            }
        }

        private void OnDisable()
        {
            // When the tank is turned off, set it to kinematic so it stops moving.
            rigidbody.isKinematic = true;

            // Stop all particle system so it "reset" it's position to the actual one instead of thinking we moved when spawning
            for(int i = 0; i < particleSystems.Length; ++i)
            {
                particleSystems[i].Stop();
            }
        }

        private void Start()
        {
            // The axes names are based on player number.
            movementAxisName = "Vertical" + playerNumber;
            turnAxisName = "Horizontal" + playerNumber;

            // Store the original pitch of the audio source.
            originalPitch = movementAudio.pitch;
        }

        private void Update()
        {
            // Store the value of both input axes.
            movementInputValue = Input.GetAxis(movementAxisName);
            turnInputValue = Input.GetAxis(turnAxisName);

            EngineAudio();
        }

        private void EngineAudio()
        {
            // If there is no input (the tank is stationary)...
            if (Mathf.Abs(movementInputValue) < 0.1f && Mathf.Abs(turnInputValue) < 0.1f)
            {
                // ... and if the audio source is currently playing the driving clip...
                if (movementAudio.clip == engineDriving)
                {
                    // ... change the clip to idling and play it.
                    movementAudio.clip = engineIdling;
                    movementAudio.pitch = Random.Range (originalPitch - pitchRange, originalPitch + pitchRange);
                    movementAudio.Play ();
                }
            }
            else
            {
                // Otherwise if the tank is moving and if the idling clip is currently playing...
                if (movementAudio.clip == engineIdling)
                {
                    // ... change the clip to driving and play.
                    movementAudio.clip = engineDriving;
                    movementAudio.pitch = Random.Range(originalPitch - pitchRange, originalPitch + pitchRange);
                    movementAudio.Play();
                }
            }
        }

        private void FixedUpdate()
        {
            // Adjust the rigidbodies position and orientation in FixedUpdate.
            Move();
            Turn();
        }

        private void Move()
        {
            // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
            Vector3 movement = transform.forward * movementInputValue * speed * Time.deltaTime;

            // Apply this movement to the rigidbody's position.
            rigidbody.MovePosition(rigidbody.position + movement);
        }

        private void Turn()
        {
            // Determine the number of degrees to be turned based on the input, speed and time between frames.
            float turn = turnInputValue * turnSpeed * Time.deltaTime;

            // Make this into a rotation in the y axis.
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

            // Apply this rotation to the rigidbody's rotation.
            rigidbody.MoveRotation(rigidbody.rotation * turnRotation);
        }
    }
}