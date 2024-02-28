namespace SpaceBattle.Lib;
using Hwdtech;
using System.Collections.Concurrent;
public class SoftStopCommand: Hwdtech.ICommand {
    private ServerThread _t;
    private BlockingCollection<Hwdtech.ICommand> _q;
    private Action _a;
    public SoftStopCommand(ServerThread t, BlockingCollection<Hwdtech.ICommand> q,Action a) {
        _t = t;
        _q = q;
        _a = a;
    }

    public void Execute() {
        Action ssbehaviour = ()=> {
            if (_q.Count()> 0) {
                var cmd = _q.Take();
                try{
                    cmd.Execute();
                } catch (Exception e) {
                    IoC.Resolve<ICommand>("ExceptionHandler.Handle", cmd, e).Execute();
                }
            } else {
                IoC.Resolve<ICommand>("Server.Commands.HardStop", _t, _a).Execute();
            }
        };
        _t.SetBehaviour(ssbehaviour);
    }
}
