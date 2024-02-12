using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState
{
    public abstract void EnterState(PlayerStateMachine state_machine);

    public abstract void ExitState(PlayerStateMachine state_machine);

    public abstract void UpdateState(PlayerStateMachine state_machine);

    public abstract void FixedUpdateState(PlayerStateMachine state_machine);

    public Vector3 Accelerate(Vector3 wish_dir, Vector3 current_velocity,
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

}
