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
    public Control TurnControl = GD.Load<PackedScene>("res://Scenes/turn_control.tscn").Instantiate<Control>();

    public LandPrefab[] landPrefabs;


    private string[] colors = new string[] { "black", "green", "blue", "purple", "pink", "orange" };
    public Texture2D[] TeamTextures;

    private bool loaded = false;

    private LandPrefab selected;
    private LandPrefab attacked;
    private int turn = 0;
    private int playersNum = 0;

    public NetworkManager NetworkManager;
    public State CurrentState;

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



        NetworkManager = GetParent().GetNode<NetworkManager>("NetworkManager");

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

        if (NetworkManager.PlayerTeam == 0)
        {
            myTurn();
        }

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
        NetworkManager.SendAttack(selected, attacked, quantity);
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
        AttackControl.GetNode<Button>("Attack1").Visible = selected.Troops > 1;
        AttackControl.GetNode<Button>("Attack2").Visible = selected.Troops > 2;
        AttackControl.GetNode<Button>("Attack3").Visible = selected.Troops > 3;
        AddSibling(AttackControl);
        CurrentState = State.Attacking;
    }

    internal void NextTurn()
    {
        turn = (turn + 1) % playersNum;
        if (turn != NetworkManager.PlayerTeam) {
            CurrentState=State.Waiting;
        } else
        {
            myTurn();
        }
    }

    private void myTurn()
    {
        AddSibling(TurnControl);
        CurrentState = State.Selecting;
    }
}

public enum State
{
    Selecting,
    Attacking,
    Waiting
}