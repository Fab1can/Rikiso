using Godot;
using System.Text.Json.Nodes;
using System.Collections.Generic;

public partial class NetworkManager : Node
{
    public enum States
    {
        Unconnected,
        Connecting,
        Connected
    }

    private WebSocketPeer ws;

    public int PlayerId;
    public List<string>[] Lands;
    public States State = States.Unconnected;
	public override void _Ready()
	{
        Lands = new List<string>[2];
        for (int i = 0; i < Lands.Length; i++)
        {
            Lands[i] = new List<string>();
        }
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
                    string data = System.Text.Encoding.Default.GetString(ws.GetPacket());
                    if (State == States.Connecting)
                    {
                        State = States.Connecting;
                        JsonNode json = JsonNode.Parse(data);
                        PlayerId = int.Parse(json["id"].ToString());
                        foreach (var lands in json["lands"].AsObject())
                        {
                            foreach (string land in lands.Value.AsArray())
                            {
                                Lands[int.Parse(lands.Key)].Add(land);
                            }
                        }
                        var modena = GetParent<SceneManager>().LoadModena();

                    }
                    else if(State==States.Connected){
                        GD.Print("ricevuto: "+data);
                    }
                }
            }
        }
        
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
