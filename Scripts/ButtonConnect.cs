using Godot;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public partial class ButtonConnect : Button
{
	private NetworkManager networkManager;
	[Export] private ButtonReady buttonReady;
    [Export] private TextEdit hostEdit;
    [Export] private TextEdit portEdit;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		networkManager = GetTree().Root.GetNode<NetworkManager>("SceneManager/NetworkManager");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public override void _Pressed()
	{
		base._Pressed();
		networkManager.Host = hostEdit.Text;
		networkManager.Port = ushort.Parse(portEdit.Text);
		Thread clientThread = new Thread(networkManager.StartClient);
		clientThread.Start();
		hostEdit.Editable = false;
		portEdit.Editable = false;
		buttonReady.Disabled = false;
		Disabled = true;
	}
}
