using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

public partial class LandLoader : Node
{

    public LandPrefab[] landPrefabs;


    private bool loaded = false;
    public Texture2D NoneTexture = GD.Load<Texture2D>("res://boxes/none.png");
    public Texture2D BorderTexture = GD.Load<Texture2D>("res://boxes/border.png");
    public Texture2D SelectedTexture = GD.Load<Texture2D>("res://boxes/selected.png");

    private string[] colors = new string[] { "black", "green", "blue", "purple", "pink", "orange" };
    private Texture2D[] teamTextures;


    private NetworkManager networkManager;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        


        teamTextures = new Texture2D[colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            teamTextures[i] = GD.Load<Texture2D>("res://boxes/"+colors[i]+".png");
        }
        networkManager = GetParent().GetNode<NetworkManager>("NetworkManager");

        Node[] children = GetChildren().Where(x => x is LandPrefab).ToArray();
        landPrefabs = new LandPrefab[children.Count()];
        string listaConfini = "";
        for (int i = 0; i < landPrefabs.Length; i++)
        {
            landPrefabs[i] = (LandPrefab)children[i];
            landPrefabs[i].LandLoader = this;
            landPrefabs[i].TeamTexture = teamTextures[networkManager.IdPlayer];
            listaConfini += "\"" + landPrefabs[i].Name + "\":["+string.Join(",", landPrefabs[i].Borders.Select(x => "\""+x.Name+"\""))+"],\n";
            
        }
        GD.Print(listaConfini);


    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void Select(LandPrefab land)
    {
        networkManager.Send(colors[networkManager.IdPlayer] + " ha selezionato " + land.Name);
        foreach (LandPrefab landp in landPrefabs)
        {
            landp.CursorTexture = NoneTexture;
        }
        land.CursorTexture = SelectedTexture;
        foreach (LandPrefab border in land.Borders)
        {
            border.CursorTexture = BorderTexture;
        }
    }
}
