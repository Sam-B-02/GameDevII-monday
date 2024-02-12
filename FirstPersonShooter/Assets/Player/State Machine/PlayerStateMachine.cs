using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{

    // State Variables
    private PlayerBaseState current_state;
    public PlayerGroundState ground_state = new PlayerGroundState();
    public PlayerAirState air_state = new PlayerAirState();

    //Player Input
    [HideInInspector] public Vector2 move_input;
    [HideInInspector] public bool jump_button_pressed = false;

    //Movement Variables
    [HideInInspector] public CharacterController character_controller;
    [HideInInspector] public Vector3 player_velocity;
    [HideInInspector] public Vector3 wish_dir = Vector3.zero;

    public float max_speed = 6f;
    public float acceleration = 60;
    public float gravity = 15f;
    public float stop_speed = 0.5f;
    public float jump_impulse = 10f;
    public float friction = 4;

    //Debug
    public TMP_Text debug_text;
    private float top_speed = 0f;

    void Start()
    {
        //Get Components
        character_controller = GetComponent<CharacterController>();

        current_state = ground_state;

        current_state.EnterState(this);
    }

    void Update()
    {
        DebugText();
        current_state.UpdateState(this);
    }

    private void FixedUpdate()
    {
        FindWishDir();
        current_state.FixedUpdateState(this);
        MovePlayer();
    }

    public void SwitchState(PlayerBaseState cur_state, PlayerBaseState new_state)
    {
        cur_state.ExitState(this);
        current_state = new_state;
        current_state.EnterState(this);
    }

    public void GetMoveInput(InputAction.CallbackContext context)
    {
        move_input = context.ReadValue<Vector2>();
    }

    public void DebugText()
    {
        //Debug
        debug_text.text = "Wish Dir: " + wish_dir.ToString();
        debug_text.text += "\nPlayer Velocity: " + player_velocity.ToString();
        debug_text.text += "\nPlayer Speed: " + new Vector3(player_velocity.x, 0, player_velocity.z).magnitude.ToString();

        // **NOT IN GUIDE** for competitive purposes
        if (new Vector3(player_velocity.x, 0, player_velocity.z).magnitude > top_speed)
            top_speed = new Vector3(player_velocity.x, 0, player_velocity.z).magnitude;
        debug_text.text += "\nHighest Speed: " + top_speed.ToString();

        debug_text.text += "\nState: " + current_state.ToString();
    }

    public void FindWishDir()
    {
        //Find wish_dir
        wish_dir = transform.right * move_input.x + transform.forward * move_input.y;
        wish_dir = wish_dir.normalized;
    }

    public void MovePlayer()
    {
        character_controller.Move(player_velocity * Time.deltaTime);
    }

    public void GetJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) jump_button_pressed = true;
        if (context.phase == InputActionPhase.Canceled) jump_button_pressed = false;
    }

}
