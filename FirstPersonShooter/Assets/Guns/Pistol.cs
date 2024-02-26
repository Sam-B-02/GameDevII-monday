using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class Pistol : MonoBehaviour
{
    public GunData gun_data;
    public Camera cam;
    private Ray ray;

    //Shooting
    private bool primary_fire_is_shooting = false;
    private bool primary_fire_hold = false;
    private float shoot_delay_timer = 0.0f;

    //Ammo Variables
    private int ammo_in_clip;

    //Debug
    public TMP_Text debug_text;

    // Start is called before the first frame update
    void Start()
    {
        //Set Variables
        ammo_in_clip = gun_data.ammo_per_clip;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(cam.transform.position, cam.transform.forward * 10000, Color.green);

        //Debug Text
        debug_text.text = "Ammo In Clip: " + ammo_in_clip.ToString();

        //Subtract from shoot timer
        if(shoot_delay_timer > 0) shoot_delay_timer -= Time.deltaTime;

        PrimaryFire();
    }

    public void GetPrimaryFireInput(InputAction.CallbackContext context)
    {
        //Checking for the initial button press
        if (context.phase == InputActionPhase.Started)
        {
            primary_fire_is_shooting = true;
        }

        //Check if the gun is automatic
        if (gun_data.automatic)
        {
            //Check if hold was completed
            if (context.interaction is HoldInteraction && context.phase == InputActionPhase.Performed)
            {
                primary_fire_hold = true;
            }
        }

        //Check if the button was released
        if (context.phase == InputActionPhase.Canceled)
        {
            primary_fire_is_shooting = false;
            primary_fire_hold = false;
        }
    }

    public void GetSecondaryFireInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) SecondaryFire();
    }

    private void PrimaryFire()
    {
        //Check if no delay
        if (shoot_delay_timer <= 0)
        {
            if (primary_fire_is_shooting || primary_fire_hold)
            {
                //Delay gun from shooting again
                shoot_delay_timer = gun_data.primary_fire_delay;

                primary_fire_is_shooting = false;

                //Set direction of the ray
                Vector3 dir = Quaternion.AngleAxis(Random.Range(-gun_data.spread, gun_data.spread), Vector3.up) * cam.transform.forward;
                dir = Quaternion.AngleAxis(Random.Range(-gun_data.spread, gun_data.spread), Vector3.right) * dir;

                //Raycast
                ray = new Ray(cam.transform.position, dir);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, gun_data.range))
                    Debug.DrawLine(transform.position, hit.point, Color.green, 0.5f);

                //Ammo
                ammo_in_clip--;
                if (ammo_in_clip <= 0) ammo_in_clip = gun_data.ammo_per_clip;
                print(ammo_in_clip);
            }
        }
    }

    private void SecondaryFire()
    {

    }
}
