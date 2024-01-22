using Godot;
using System;

public partial class SceneManager : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public LandLoader LoadModena()
	{
		RemoveChild(GetNode("Menu"));
		Control ui = (Control)GD.Load<PackedScene>("res://Scenes/UI.tscn").Instantiate();
		AddChild(ui);
		LandLoader modena = (LandLoader)GD.Load<PackedScene>("res://Scenes/land_loader_modena.tscn").Instantiate();
		AddChild(modena);
		ui.MoveToFront();
		return modena;
	}
}
