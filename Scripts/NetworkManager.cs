using Godot;
using System.Text.Json.Nodes;
using System.Collections.Generic;
using System.Text.Json;
using System;
using System.IO;
using static Godot.Projection;

public partial class NetworkManager : Node
{
    public enum States
    {
        Unconnected,
        Connecting,
        Connected
    }

    private WebSocketPeer ws;

    private LandLoader landLoader;

    public int PlayerTeam;
    public Dictionary<string, (int Team, int Troops)> StartLands;
    public States State = States.Unconnected;
	public override void _Ready()
	{
        base._Ready();
        StartLands = new Dictionary<string, (int team, int troops)>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if(State!=States.Unconnected)
        {
            ws.Poll();
            var state = ws.GetReadyState();
            if (state == WebSocketPeer.State.Open)
            {
                while (ws.GetAvailablePacketCount() > 0)
                {
                    JsonNode json = JsonNode.Parse(System.Text.Encoding.Default.GetString(ws.GetPacket()));
                    if (State == States.Connecting)
                    {
                        onConnected(json);

                    }
                    else if(State==States.Connected){
                        GD.Print(json.ToString());
                        string cmd = json["cmd"].ToString();
                        switch (cmd)
                        {
                            case "attack":
                                OnAttack(json["lands"]);
                                break;
                            case "turn":
                                onTurn(int.Parse(json["troops"].ToString()));
                                break;
                            case "place":
                                onPlace(json["lands"]);
                                break;
                        }
                    }
                }
            }
        }
        
    }

    private void onPlace(JsonNode lands)
    {
        landLoader.UpdateLands(getLands(lands.AsObject()));
    }

    private void onTurn(int troops)
    {
        landLoader.NextTurn(troops);
    }

    private void OnAttack(JsonNode lands)
    {
        landLoader.UpdateLands(getLands(lands.AsObject()));
    }

    private Dictionary<string, (int Team, int Troops)> getLands(JsonObject jObj)
    {
        Dictionary<string, (int Team, int Troops)> lands = new Dictionary<string, (int Team, int Troops)> ();
        var landsEnumerator = jObj.GetEnumerator();
        while (landsEnumerator.MoveNext())
        {
            var land = landsEnumerator.Current.Key;
            var team = int.Parse(landsEnumerator.Current.Value.AsObject()["team"].ToString());
            var troops = int.Parse(landsEnumerator.Current.Value.AsObject()["troops"].ToString());
            lands.Add(land, (team, troops));
        }
        return lands;
    }

    private void onConnected(JsonNode json)
    {
        State = States.Connected;
        PlayerTeam = int.Parse(json["team"].ToString());
        var lands = json["lands"].AsObject();
        StartLands = getLands(lands);
        landLoader = GetParent<SceneManager>().LoadModena();
    }

    public void SendAttack(LandPrefab attacker, LandPrefab attacked, int troops) {
        var data = new Dictionary<string, object>
        {
            { "cmd","attack" },
            { "from",attacker.Name.ToString() },
            {"to",attacked.Name.ToString() },
            {"with",troops }
        };
        Send(JsonSerializer.Serialize(data));
    }

    public void StartClient()
    {
        if (!File.Exists("address.txt"))
        {
            using (StreamWriter outputFile = new StreamWriter("address.txt"))
            {
                outputFile.WriteLine("127.0.0.1");
            }
        }
        
        string ip = File.ReadAllLines("address.txt")[0];
        int port = 18000;

        ws = new WebSocketPeer();
        ws.ConnectToUrl("ws://" + ip + ":" + port);
        State = States.Connecting;
    }

    public void Send(string data)
    {
        ws.SendText(data);
    }

    internal void SendTurn()
    {
        var data = new Dictionary<string, object>
        {
            { "cmd","turn" }
        };
        Send(JsonSerializer.Serialize(data));
    }

    internal void SendReady(bool value)
    {
        var data = new Dictionary<string, object>
        {
            { "cmd","ready" },
            { "value" , value }
        };
        Send(JsonSerializer.Serialize(data));
    }

    internal void Place(LandPrefab land)
    {
        var data = new Dictionary<string, object>
        {
            { "cmd","place" },
            { "land" , land.Name.ToString() }
        };
        Send(JsonSerializer.Serialize(data));
    }
}
