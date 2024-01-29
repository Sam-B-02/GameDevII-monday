using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using TMPro.EditorUtilities;

public class PlayerController : MonoBehaviour
{

    //Camera Variables
    public Camera cam;
    private Vector2 look_input = Vector2.zero;
    private float look_speed = 60f;
    private float horizontal_look_angle = 0f;

    public bool invert_x = false;
    public bool invert_y = false;
    private int invert_factor_x = 1;
    private int invert_factor_y = 1;

    [Range(0.01f, 1f)] public float sensitivity;

    //Player Input
    private Vector2 move_input;
    private bool grounded;

    //Movement Variables
    private CharacterController character_controller;
    private Vector3 player_velocity;
    private Vector3 wish_dir = Vector3.zero;

    public float max_speed = 6f;
    public float acceleration = 60;
    public float gravity = 15f;
    public float stop_speed = 0.5f;
    public float jump_impulse = 10f;
    public float friction = 4;

    //Debug
    public TMP_Text debug_text;

    // Start is called before the first frame update
    void Start()
    {
        //Hide the mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Inverting Camera
        if (invert_x) invert_factor_x = -1;
        if (invert_y) invert_factor_y = -1;

        //Get reference to the player controller
        character_controller = GetComponent<CharacterController>();
    }

    private float top_speed = 0f;

    // Update is called once per frame
    void Update()
    {
        //Debug
        debug_text.text = "Wish Dir: " + wish_dir.ToString();
        debug_text.text += "\nPlayer Velocity: " + player_velocity.ToString();
        debug_text.text += "\nPlayer Speed: " + new Vector3(player_velocity.x, 0, player_velocity.z).magnitude.ToString();

        if (new Vector3(player_velocity.x, 0, player_velocity.z).magnitude > top_speed)
            top_speed = new Vector3(player_velocity.x, 0, player_velocity.z).magnitude;
        debug_text.text += "\nHighest Speed: " + top_speed.ToString();
        debug_text.text += "\nGrounded: " + grounded.ToString();

        Look();
    }

    private void FixedUpdate()
    {
        //Find wish_dir
        wish_dir = transform.right * move_input.x + transform.forward * move_input.y;
        wish_dir = wish_dir.normalized;

        //Move player if on ground or in air
        grounded = character_controller.isGrounded;
        if (grounded)
        {
            player_velocity = MoveGround(wish_dir, player_velocity);
        }
        else
        {
            player_velocity = MoveAir(wish_dir, player_velocity);
        }

        //Gravity
        player_velocity.y -= gravity * Time.deltaTime;
        if (grounded && player_velocity.y < 0) //Cap y vel on the ground
        {
            player_velocity.y = -2;
        }

        character_controller.Move(player_velocity * Time.deltaTime);
    }

    public void GetLookInput(InputAction.CallbackContext context)
    {
        look_input = context.ReadValue<Vector2>();
    }
    public void GetMoveInput(InputAction.CallbackContext context) 
    {
        move_input = context.ReadValue<Vector2>();
    }
    public void GetJumpInput(InputAction.CallbackContext context)
    {
        Jump();
    }

    public void Jump()
    {
        if (grounded)
        {
            player_velocity.y = jump_impulse;
        }
    }

    private Vector3 Accelerate(Vector3 wish_dir, Vector3 current_velocity, 
        float accel, float max_speed)
    {
        //Vector projection of the current velocity onto the wish_dir,
        //the speed the player is going
        float proj_speed = Vector3.Dot(current_velocity, wish_dir);
        //The acceleration component to add to the projected speed
        float accel_speed = accel * Time.deltaTime;

        //If necessary, truncate the accelerated velocity so the vector
        //projection does not exceed max_speed
        if (proj_speed + accel_speed > max_speed)
            accel_speed = max_speed - proj_speed;

        //Return the new speed
        return current_velocity + (wish_dir * accel_speed);
    }
    private Vector3 MoveGround(Vector3 wish_dir, Vector3 current_velocity)
    {
        //Create new velocity vector
        Vector3 new_velocity = new Vector3(current_velocity.x, 0, current_velocity.z);
        //Remove the y component for now

        //Since on ground, apply friction
        float speed = new_velocity.magnitude;
        if (speed <= stop_speed)
        {
            new_velocity = Vector3.zero;
            speed = 0;
        }

        if (speed != 0)
        {
            float drop = speed * friction * Time.deltaTime;
            new_velocity *= Mathf.Max(speed - drop, 0) / speed;
            //Scale velocity based on friction
        }
        new_velocity = new Vector3(new_velocity.x, current_velocity.y, new_velocity.z);
        //Add y component back in

        return Accelerate(wish_dir, new_velocity, acceleration, max_speed);
    }
    private Vector3 MoveAir(Vector3 wish_dir, Vector3 current_velocity)
    {
        return Accelerate(wish_dir, current_velocity, acceleration, max_speed);
    }

    private void Look()
    {
        //Left/Right
        transform.Rotate(Vector3.up, look_input.x * look_speed * 
            Time.deltaTime * invert_factor_x * sensitivity);

        //Up/Down
        float angle = look_input.y * look_speed * Time.deltaTime * 
            invert_factor_y * sensitivity;
        horizontal_look_angle -= angle;
        horizontal_look_angle = Mathf.Clamp(horizontal_look_angle, -90, 90);
        cam.transform.localRotation = Quaternion.Euler(horizontal_look_angle, 0, 0);
    }

}
