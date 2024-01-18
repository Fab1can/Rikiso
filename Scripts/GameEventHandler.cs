using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class GameEventHandler
{
    public static GameEventHandler Instance = new GameEventHandler();
    private GameEventHandler() { }

    public event EventHandler<int> OnAttackPressed;
    public event EventHandler OnTurnPressed;

    public void EmitAttackPressed(int quantity)
    {
        OnAttackPressed?.Invoke(this, quantity);
    }

    public void EmitTurnPressed()
    {
        OnTurnPressed?.Invoke(this, null);
    }
}
