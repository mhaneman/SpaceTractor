using Godot;
using System;

public class Bike : VehicleBody
{
    Vector3 jump = new Vector3(-0.1f, 500f, 0.1f);

    ScreenControls controls;
    public override void _Ready()
    {
        controls = GetNode<ScreenControls>("ScreenControls");
        controls.Connect("userAction", this, "on_userAction");
    }

    public override void _PhysicsProcess(float delta)
    {
        this.Steering = Input.GetAxis("left", "right") * 0.1f;
        this.EngineForce = -100f;
        
        if (Input.IsActionJustPressed("jump"))
        {
            this.ApplyCentralImpulse(jump);
        }

        if (this.GlobalTransform.origin.y < -10f || 
            this.GlobalTransform.origin.x < -10f ||
            this.GlobalTransform.origin.x > 10f)
        {
            GD.Print("Death Boundary");
        }
    }

    private void on_userAction(string direction)
    {
        GD.Print(direction);
    }
}
