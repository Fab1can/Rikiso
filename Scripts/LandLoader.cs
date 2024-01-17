using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.IO;
using System.Linq;

public partial class LandLoader : Node
{
    public Control AttackControl = GD.Load<PackedScene>("res://Scenes/attack_control.tscn").Instantiate<Control>();

    public LandPrefab[] landPrefabs;


    private string[] colors = new string[] { "black", "green", "blue", "purple", "pink", "orange" };
    public Texture2D[] TeamTextures;

    private bool loaded = false;

    private LandPrefab selected;
    private LandPrefab attacked;

    public NetworkManager NetworkManager;
    public State CurrentState = State.Selecting;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        GameEventHandler.Instance.OnAttackPressed += HandleAttackPressed;
        TeamTextures = new Texture2D[colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            TeamTextures[i] = GD.Load<Texture2D>("res://Textures/Box/Teams/" + colors[i] + ".png");
        }



        NetworkManager = GetParent().GetNode<NetworkManager>("NetworkManager");

        Node[] children = GetChildren().Where(x => x is LandPrefab).ToArray();
        landPrefabs = new LandPrefab[children.Count()];
        string listaTerritori = "";
        for (int i = 0; i < landPrefabs.Length; i++)
        {
            landPrefabs[i] = (LandPrefab)children[i];
            landPrefabs[i].LandLoader = this;
            landPrefabs[i].Team = NetworkManager.StartLands[landPrefabs[i].Name].Team;
            landPrefabs[i].Troops = NetworkManager.StartLands[landPrefabs[i].Name].Troops;
            listaTerritori += "\"" + landPrefabs[i].Name + "\": { \"borders\" : ["+string.Join(",", landPrefabs[i].Borders.Select(x => "\""+x.Name+"\""))+"] },\n";
            
        }
        GD.Print(listaTerritori);


    }

    private void HandleAttackPressed(object sender, int quantity)
    {
        NetworkManager.Attack(selected, attacked, quantity);
        UnSelect();
        CurrentState = State.Selecting;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void Select(LandPrefab land)
    {
        foreach (LandPrefab landp in landPrefabs)
        {
            landp.Selection = Selection.NotSelected;
        }
        land.Selection = Selection.Selected;
        selected = land;
        foreach (LandPrefab border in land.Borders)
        {
            if(land.Team != border.Team)
            {
                border.Selection = Selection.Border;
            }
        }
    }

    public void UnSelect()
    {
        foreach (LandPrefab landp in landPrefabs)
        {
            landp.Selection = Selection.NotSelected;
        }
        selected = null;
    }

    public void Attack(LandPrefab land)
    {
        attacked = land;
        AddSibling(AttackControl);
        CurrentState = State.Attacking;
    }
}

public enum State
{
    Selecting,
    Attacking
}