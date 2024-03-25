namespace SpaceBattle.Lib;

public class HardStopCommand : Hwdtech.ICommand
{
    private readonly ServerThread _t;
    public HardStopCommand(ServerThread t)
    {
        _t = t;
    }

    public void Execute()
    {
        _t.Stop();
    }
}
