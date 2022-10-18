using Godot;
using System;

public class World : Spatial
{
    VehicleBody bike;
    float resetFloatDist = 1000f;
    public override void _Ready()
    {
        bike = GetNode<VehicleBody>("Bike");    
    }

    public override void _Process(float delta)
    {
        if (bike.GlobalTransform.origin.z < -resetFloatDist)
            ResetPosition();
    }

    private void ResetPosition()
    {
        GD.Print("reset");
        foreach(var i in this.GetChildren())
            this.GlobalTranslate(Vector3.Back * resetFloatDist);
    }


}
