using Godot;
using System;
using System.Net.Sockets;
using System.Net.Http;
using static Godot.HttpRequest;

public partial class NetworkManager : Node
{
    public enum States
    {
        Unconnected,
        Waiting,
        Connecting,
        Connected
    }

    private WebSocketPeer ws;

    public int IdPlayer;
    public States State = States.Unconnected;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (State==States.Connecting)
        {
            State = States.Connected;
            GetParent<SceneManager>().LoadModena();
        }
        if(State!=States.Unconnected)
        {
            ws.Poll();
            var state = ws.GetReadyState();
            if (state == WebSocketPeer.State.Open)
            {
                while (ws.GetAvailablePacketCount() > 0)
                {
                    string data = System.Text.Encoding.Default.GetString(ws.GetPacket());
                    if (State == States.Waiting)
                    {
                        State = States.Connecting;
                        IdPlayer = int.Parse(data);
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
        State = States.Waiting;
    }

    public void Send(string data)
    {
        ws.SendText(data);
    }
}
