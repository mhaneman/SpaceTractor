using Godot;
using System;

public class ScreenControls : Node
{
    [Signal] delegate void userAction(string direction);
    Vector2 screenRes;

    public override void _Ready() 
    {
        screenRes = OS.WindowSize;
    }

	public override void _UnhandledInput(InputEvent @event) {
		if (@event is InputEventScreenTouch eventKey) 
		{
			if (eventKey.Pressed)
			{
				Vector2 clickPos = eventKey.Position;
				if (clickPos.x > screenRes.x / 2)
					EmitSignal("userAction", "right");
				else
					EmitSignal("userAction", "left");
			}
		}
	
	}
}
