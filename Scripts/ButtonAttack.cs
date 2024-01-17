using Godot;
using System;

public partial class ButtonAttack : Button
{
	[Export]
	private int Quantity;

    public override void _Pressed()
    {
        base._Pressed();
        GameEventHandler.Instance.EmitAttackPressed(Quantity);
        GetParent().GetParent().RemoveChild(GetParent());
    }
}
