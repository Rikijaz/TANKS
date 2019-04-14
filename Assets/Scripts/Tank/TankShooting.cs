﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TankShooting : MonoBehaviour
{
    public class MissleData
    {
        public Vector3 destination { get; private set; }
        public float lifeSpan { get; private set; }

        public MissleData(Vector3 destination, float lifeSpan)
        {
            this.destination = destination;
            this.lifeSpan = lifeSpan;
        }

        public void UpdateLifeSpan()
        {
            lifeSpan -= Time.deltaTime;
        }
    }

    public int playerNumber = 1;       
    public Rigidbody shell;            
    public Transform fireTransform;    
    public Slider aimSlider;           
    public AudioSource shootingAudio;  
    public AudioClip chargingClip;     
    public AudioClip fireClip;         
    public float minLaunchForce = 15f; 
    public float maxLaunchForce = 30f; 
    public float maxChargeTime = 0.75f;
    public float currentLaunchForce { get; private set; }
    public List<MissleData> missleDataCache { get; private set; }

    private const string FireInputName = "Fire";
    private const string AITag = "AI";
    private const float shellRadius = 0.15f;
    private const float groundLevel = 0f;

    private string fireButton;
    private float chargeSpeed;
    private bool fired;
    
    private void OnEnable()
    {
        currentLaunchForce = minLaunchForce;
        aimSlider.value = minLaunchForce;
    }

    private void Start()
    {
        fireButton = FireInputName + playerNumber;
        missleDataCache = new List<MissleData>();
        chargeSpeed = (maxLaunchForce - minLaunchForce) / maxChargeTime;
    }


    // Track the current state of the fire button and make decisions based on 
    // the current launch force.
    private void Update()
    {
        UpdateMissleDataCache();
        UpdateUI();

        if ((currentLaunchForce >= maxLaunchForce) && (!fired))
        {          
            Fire();
        }
        else if (Input.GetButtonDown(fireButton))
        {
            PrepareToFire();
        }
        else if (Input.GetButton(fireButton) && !fired)
        {
            ChargeMissle();
        }
        else if (Input.GetButtonUp(fireButton) && !fired)
        {
            Fire();
        }
    }

    public void UpdateUI()
    {
        aimSlider.value = minLaunchForce;
    }

    public void PrepareToFire()
    {
        fired = false;
        currentLaunchForce = minLaunchForce;

        shootingAudio.clip = chargingClip;
        shootingAudio.Play();
    }

    public void ChargeMissle()
    {
        currentLaunchForce += chargeSpeed * Time.deltaTime;
        aimSlider.value = currentLaunchForce;
    }

    // Instantiate and launch the shell.
    public void Fire()
    {
        currentLaunchForce = Mathf.Clamp(
            currentLaunchForce, 
            minLaunchForce, 
            maxLaunchForce);

        fired = true;

        Rigidbody shellInstance = Instantiate(
            shell,
            fireTransform.position,
            fireTransform.rotation) as Rigidbody;

        shellInstance.velocity = currentLaunchForce * fireTransform.forward;

        missleDataCache.Add(GetMissleData(shellInstance));

        shootingAudio.clip = fireClip;
        shootingAudio.Play();

        currentLaunchForce = minLaunchForce;
    }

    /// <summary>
    /// Update the missle's lifeSpan. If lifeSpan has expired (negative), then
    /// remove the missle data
    /// </summary>
    private void UpdateMissleDataCache()
    {
        for (var i = 0; i < missleDataCache.Count; ++i)
        {
            missleDataCache[i].UpdateLifeSpan();

            if (missleDataCache[i].lifeSpan <= 0)
            {
                missleDataCache.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Assuming that the missle will land on the ground (y = 0f), determine its
    /// destination
    /// </summary>
    private MissleData GetMissleData(Rigidbody shellRigidbody)
    {
        float time = CalculateMissleTimeOfFlight(shellRigidbody, groundLevel);

        float x = transform.position.x + shellRigidbody.velocity.x * time;
        float z = transform.position.z + shellRigidbody.velocity.z * time;

        Vector3 missleDestination = new Vector3(x, groundLevel, z);
        MissleData missleData = new MissleData(missleDestination, time);
        UpdateDestinationIfHitObstacle(ref missleData, shellRigidbody);

        return missleData;
    }

    /// <summary>
    /// Given the missle's destination, if there is an obstacle (that is not the 
    /// AI) between said position and firing position, update the missle's 
    /// destination to where it will collide with the obstacle
    /// </summary>
    private void UpdateDestinationIfHitObstacle(ref MissleData missleData, Rigidbody missle)
    {
        bool isObstacleBetweenDestination = false;

        RaycastHit obstacleHit;

        isObstacleBetweenDestination = Physics.Linecast(
            missle.transform.position, 
            missleData.destination, 
            out obstacleHit);

        if (isObstacleBetweenDestination && (obstacleHit.collider.tag != AITag))
        {
            float updatedLifeSpan = CalculateMissleTimeOfFlight(
                missle, 
                obstacleHit.point.y);

            MissleData updatedMissleData = new MissleData(
                obstacleHit.point,
                updatedLifeSpan);

            missleData = updatedMissleData;
        }
    }

    private float CalculateMissleTimeOfFlight(Rigidbody missle, float yLanding)
    {
        float height = yLanding - (missle.transform.position.y + shellRadius);
        float gravity = Physics.gravity.magnitude;
        float yVelocity = missle.velocity.y;

        float time = (yVelocity / gravity) + Mathf.Sqrt(
            (Mathf.Pow(yVelocity, 2) / Mathf.Pow(gravity, 2)) -
            (2 * height / gravity));

        return time;
    }
}