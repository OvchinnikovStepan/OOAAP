namespace SpaceBattle.Lib;
using Hwdtech;
public class SoftStopCommand : Hwdtech.ICommand
{
    private readonly ServerThread _t;
    private readonly Action _a;
    public SoftStopCommand(ServerThread t, Action a)
    {
        _t = t;
        _a = a;
    }

    public void Execute()
    {
        if (_t.Equals(Thread.CurrentThread))
        {
            var old_behaviour = _t.GetBehaviour();
            Action ssbehaviour = () =>
            {
                if (_t.IsNotEmpty())
                {
                    old_behaviour();
                }
                else
                {
                    IoC.Resolve<Hwdtech.ICommand>("Server.Commands.HardStop", _t, _a).Execute();
                }
            };
            _t.SetBehaviour(ssbehaviour);
        }
    }
}
