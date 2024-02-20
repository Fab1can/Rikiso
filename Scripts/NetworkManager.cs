using Godot;
using System.Text.Json.Nodes;
using System.Collections.Generic;
using System.Text.Json;
using System;
using System.IO;
using static Godot.Projection;
using System.Linq;
using static Godot.OpenXRHand;

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

    public string Host;
    public ushort Port;
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
                                OnAttack(json["dicesAtt"].AsArray(), json["dicesDef"].AsArray(), json["conquer"].AsValue().GetValue<bool>(), json["lands"]);
                                break;
                            case "turn":
                                onTurn(json["troops"].AsValue().GetValue<int>(), json["pre"].AsValue().GetValue<bool>());
                                break;
                            case "place":
                                onPlace(json["lands"]);
                                break;
                            case "move":
                                onMove(json["lands"]);
                                break;
                        }
                    }
                }
            }
        }
        
    }

    private void onMove(JsonNode lands)
    {
        landLoader.UpdateLands(getLands(lands.AsObject()));
    }

    private void onPlace(JsonNode lands)
    {
        landLoader.UpdateLands(getLands(lands.AsObject()));
    }

    private void onTurn(int troops, bool pre)
    {
        landLoader.NextTurn(troops, pre);
    }

    private void OnAttack(JsonArray dicesAtt, JsonArray dicesDef, bool conquer, JsonNode lands)
    {
        string eventText = "Dadi attacco: "+ string.Join(", ", dicesAtt)+"\nDadi difesa: "+string.Join(", ", dicesDef)+(conquer?"\nTerritorio conquistato":"");
        
        landLoader.NotifyEvent(eventText);
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

        ws = new WebSocketPeer();
        ws.ConnectToUrl("ws://" + Host + ":" + Port);
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
