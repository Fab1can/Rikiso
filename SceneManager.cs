using Godot;
using System;

public partial class SceneManager : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void LoadModena()
	{
		RemoveChild(GetNode("Menu"));
        LandLoader modena = (LandLoader)GD.Load<PackedScene>("res://land_loader_modena.tscn").Instantiate();
		AddChild(modena);
        Control ui = (Control)GD.Load<PackedScene>("res://ui.tscn").Instantiate();
        AddChild(ui);
    }
}
