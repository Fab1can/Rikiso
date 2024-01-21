using Godot;
using System;

public partial class ButtonTurn : Button
{
    public override void _Pressed()
    {
        base._Pressed();
        GameEventHandler.Instance.EmitTurnPressed();
        Hide();

    }
}
