using Godot;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public partial class ServerButton : Button
{
	[Export]
	public NetworkManager NetworkManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        
	}

    public override void _Pressed()
    {
        base._Pressed();
        Thread clientThread = new Thread(NetworkManager.StartClient);
        clientThread.Start();
    }
}
