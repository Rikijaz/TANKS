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

    /*
    private string fireButton;         
    private float currentLaunchForce;  
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
    */

    private void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.
    }


    private void Fire()
    {
        // Instantiate and launch the shell.
    }
}