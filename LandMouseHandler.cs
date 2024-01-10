using Godot;
using System;

public partial class LandMouseHandler : Node
{
    public LandPrefab Land { get; private set; }
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var p = GetParent();
        Land = GetParent<LandPrefab>();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {

        if (@event is InputEventMouseButton mouse)
        {
            int buttonId = (int)mouse.ButtonIndex;
			if(buttonId == (int)MouseButton.Left && mouse.Pressed)
            {
                if (Land.Rect.HasPoint(Land.ToLocal(mouse.Position)))
                {
                    Land.LandLoader.Select(Land);
                }
            }
		}
    }
}
