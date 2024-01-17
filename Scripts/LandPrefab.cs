using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

[Tool]
public partial class LandPrefab : Node2D
{

    public static Texture2D NoneTexture = GD.Load<Texture2D>("res://Textures/Box/Cursors/none.png");
    public static Texture2D BorderTexture = GD.Load<Texture2D>("res://Textures/Box/Cursors/border.png");
    public static Texture2D SelectedTexture = GD.Load<Texture2D>("res://Textures/Box/Cursors/selected.png");


    public LandLoader LandLoader { get; set; }
    public LandMouseHandler MouseHandler { get; private set; }

    public Texture2D TeamTexture { get => GetChild<Sprite2D>(1).Texture; private set => GetChild<Sprite2D>(1).Texture = value; }
    public Texture2D CursorTexture { get => GetChild<Sprite2D>(2).Texture; private set => GetChild<Sprite2D>(2).Texture = value; }

    private int troops = 1;
    public int Troops { get => troops; set {
            troops = value;
            GetNode<Label>("Control/Troops").Text = troops.ToString();
        }
    }

    private Array<LandPrefab> borders = new Array<LandPrefab>();
    private Selection selection = Selection.NotSelected;
    public Selection Selection { get => selection; set { 
            selection = value;
            switch (selection)
            {
                case Selection.NotSelected:
                    CursorTexture = NoneTexture; break;
                case Selection.Selected:
                    CursorTexture = SelectedTexture; break;
                case Selection.Border:
                    CursorTexture = BorderTexture; break;
            }
        }
    }

    private int team;

    public int Team { get => team; set
        {
            team = value;
            TeamTexture= LandLoader.TeamTextures[team];
        } 
    }

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
public enum Selection
{
    NotSelected,
    Selected,
    Border
}