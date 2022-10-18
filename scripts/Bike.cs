using Godot;
using System;

public class Bike : VehicleBody
{
    Vector3 jump = new Vector3(-0.1f, 400f, 0.1f);

    ScreenControls controls;
    RayCast rayOnFloor;
    public override void _Ready()
    {
        rayOnFloor = GetNode<RayCast>("RayCast");
        controls = GetNode<ScreenControls>("ScreenControls");
        controls.Connect("userAction", this, "on_userAction");
        this.LinearVelocity = new Vector3(0f, 0f, -25f);
    }

    public override void _PhysicsProcess(float delta)
    {
        this.Steering = Input.GetAxis("left", "right") * 0.2f;
        this.EngineForce = -100f;
        
        if (Input.IsActionJustPressed("jump") && rayOnFloor.IsColliding())
        {
            this.ApplyCentralImpulse(jump);
        }

        if (this.GlobalTransform.origin.y < -10f)
        {
            GD.Print("Death Boundary");
        }
    }

    private void on_userAction(string direction)
    {
        GD.Print(direction);
    }
}
