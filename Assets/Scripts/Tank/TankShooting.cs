using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
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
        fireButton = "Fire" + playerNumber;

        chargeSpeed = (maxLaunchForce - minLaunchForce) / maxChargeTime;
    }


    // Track the current state of the fire button and make decisions based on the current launch force.
    private void Update()
    {
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

        shootingAudio.clip = fireClip;
        shootingAudio.Play();

        currentLaunchForce = minLaunchForce;
    }
}