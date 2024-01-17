using Godot;
using System.Text.Json.Nodes;
using System.Collections.Generic;
using System.Text.Json;

public partial class NetworkManager : Node
{
    public enum States
    {
        Unconnected,
        Connecting,
        Connected
    }

    private WebSocketPeer ws;

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

                    }
                }
            }
        }
        
    }

    private void onConnected(JsonNode json)
    {
        State = States.Connected;
        PlayerTeam = int.Parse(json["team"].ToString());
        var lands = json["lands"].AsObject();
        var _lands = lands.GetEnumerator();
        while (_lands.MoveNext()) 
        {
            var land = _lands.Current.Key;
            var team = int.Parse(_lands.Current.Value.AsObject()["team"].ToString());
            var troops = int.Parse(_lands.Current.Value.AsObject()["troops"].ToString());
            StartLands.Add(land, (team, troops));
        } 
        var modena = GetParent<SceneManager>().LoadModena();
    }

    public void Attack(LandPrefab attacker, LandPrefab attacked, int troops) {
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
        string ip = "127.0.0.1";
        int port = 1337;

        ws = new WebSocketPeer();
        ws.ConnectToUrl("ws://" + ip + ":" + port);
        State = States.Connecting;
    }

    public void Send(string data)
    {
        ws.SendText(data);
    }
}
