using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

[Tool]
public partial class LandPrefab : Node2D
{
    public LandLoader LandLoader { get; set; }
    public LandMouseHandler MouseHandler { get; private set; }

    public Texture2D TeamTexture { get => GetChild<Sprite2D>(1).Texture; set => GetChild<Sprite2D>(1).Texture = value; }
    public Texture2D CursorTexture { get => GetChild<Sprite2D>(2).Texture; set => GetChild<Sprite2D>(2).Texture = value; }

    private Array<LandPrefab> borders = new Array<LandPrefab>();

    [Export]
    public Array<LandPrefab> Borders
    {
        get => borders;
        set
        {
            foreach (LandPrefab b in borders)
            {
                if (b != null && b.Borders.Contains(this) && !value.Contains(b))
                {
                    b.Borders.Remove(this);
                    GD.Print(b.Name + " remove " + Name);
                }
            }
            borders = value;
            foreach (LandPrefab b in borders)
            {
                if (b != null && !b.Borders.Contains(this))
                {
                    b.Borders.Add(this);
                    GD.Print(b.Name + " add " + Name);
                }
            }
        }
    }

    public Rect2 Rect { get => GetChild<Sprite2D>(1).GetRect(); }



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        MouseHandler = (LandMouseHandler)GetChild(0);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

}