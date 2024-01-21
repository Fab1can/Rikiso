using Godot;
using System;
using System.Threading;

public partial class ButtonReady : Button
{
    private NetworkManager networkManager;
    private bool ready = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        networkManager = GetTree().Root.GetNode<NetworkManager>("SceneManager/NetworkManager");
    }


    public override void _Pressed()
    {
        base._Pressed();
        ready = !ready;
        if(ready)
        {
            Text = "Non pronto";
        }
        else
        {
            Text = "Pronto";
        }

        networkManager.SendReady(ready);
    }
}
