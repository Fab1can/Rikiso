using Godot;
using System;

public partial class LandMouseHandler : Button
{
    public LandPrefab Land { get; private set; }
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Land = GetParent<LandPrefab>();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Pressed()
    {
        base._Pressed();
        if (Land.LandLoader.CurrentState != State.Selecting && Land.LandLoader.CurrentState != State.Placing) { return; }
        if (Land.Selection == Selection.NotSelected && Land.LandLoader.NetworkManager.PlayerTeam == Land.Team)
        {
            if (Land.LandLoader.CurrentState == State.Selecting)
            {
                Land.LandLoader.Select(Land);
            }
            else
            {
                Land.LandLoader.Place(Land);
            }

        }
        if (Land.Selection == Selection.Border)
        {
            Land.LandLoader.Attack(Land);
        }
    }
}
