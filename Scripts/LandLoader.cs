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
    private int turn = -1;
    private int playersNum = 0;
    private int troopsToPlace;

    public NetworkManager NetworkManager;
    public State CurrentState = State.Waiting;

    private Control UI;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        GameEventHandler.Instance.OnAttackPressed += HandleAttackPressed;
        GameEventHandler.Instance.OnTurnPressed += HandleTurnPressed;
        TeamTextures = new Texture2D[colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            TeamTextures[i] = GD.Load<Texture2D>("res://Textures/Box/Teams/" + colors[i] + ".png");
        }

        UI = GetParent().GetNode<Control>("UI");

        NetworkManager = GetParent().GetNode<NetworkManager>("NetworkManager");

        UI.GetNode<Label>("Team").Text = colors[NetworkManager.PlayerTeam];

        Node[] children = GetChildren().Where(x => x is LandPrefab).ToArray();
        landPrefabs = new LandPrefab[children.Count()];
        string listaTerritori = "";
        for (int i = 0; i < landPrefabs.Length; i++)
        {
            landPrefabs[i] = (LandPrefab)children[i];
            landPrefabs[i].LandLoader = this;
            landPrefabs[i].Team = NetworkManager.StartLands[landPrefabs[i].Name].Team;
            if (landPrefabs[i].Team>playersNum) { playersNum = landPrefabs[i].Team; }
            landPrefabs[i].Troops = NetworkManager.StartLands[landPrefabs[i].Name].Troops;
            listaTerritori += "\"" + landPrefabs[i].Name + "\": { \"borders\" : ["+string.Join(",", landPrefabs[i].Borders.Select(x => "\""+x.Name+"\""))+"] },\n";
            
        }
        playersNum++;
        GD.Print(listaTerritori);
    }

    private void HandleTurnPressed(object sender, EventArgs e)
    {
        NetworkManager.SendTurn();
    }

    public void UpdateLands(System.Collections.Generic.Dictionary<string, (int Team, int Troops)> lands)
    {
        for (int i = 0; i < landPrefabs.Length; i++)
        {
            landPrefabs[i].Team = lands[landPrefabs[i].Name].Team;
            landPrefabs[i].Troops = lands[landPrefabs[i].Name].Troops;
        }
    }

    private void HandleAttackPressed(object sender, int quantity)
    {
        if(quantity > 0)
        {
            NetworkManager.SendAttack(selected, attacked, quantity);
        }
        UnSelect();
        UI.GetNode<Button>("Turn").Show();
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
        UI.GetNode<Button>("Turn").Hide();
        AttackControl.GetNode<Button>("Attack1").Visible = selected.Troops > 1;
        AttackControl.GetNode<Button>("Attack2").Visible = selected.Troops > 2;
        AttackControl.GetNode<Button>("Attack3").Visible = selected.Troops > 3;
        AddSibling(AttackControl);
        CurrentState = State.Attacking;
    }

    internal void NextTurn(int troops)
    {
        UnSelect();
        turn = (turn + 1) % playersNum;
        if (turn != NetworkManager.PlayerTeam) {
            CurrentState=State.Waiting;
            UI.GetNode<Label>("YourTurn").Hide();
        } else
        {
            troopsToPlace = troops;
            myTurn();
        }
    }

    private void myTurn()
    {
        UI.GetNode<Label>("YourTurn").Show();
        if (troopsToPlace > 0)
        {
            CurrentState = State.Placing;
        }
        else
        {
            CurrentState = State.Selecting;
        }
    }

    internal void Place(LandPrefab land)
    {
        land.Troops++;
        troopsToPlace--;
        NetworkManager.Place(land);
        if(troopsToPlace == 0)
        {
            CurrentState = State.Selecting;
            UI.GetNode<Button>("Turn").Show();
        }
    }

    internal void Place(string land)
    {
        
    }
}

public enum State
{
    Selecting,
    Attacking,
    Waiting,
    Placing
}