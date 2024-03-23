using Hwdtech;
using System.Collections.Concurrent;

namespace SpaceBattle.Lib;

public class InitSendCommandCmd: Hwdtech.ICommand
{
    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.SendCommand", (object[] args)=> {
            var id = (int)args[0];
            var cmd = (ICommand)args[1];
            var q=IoC.Resolve<BlockingCollection<Hwdtech.ICommand>>("Server.Command.GetThreadQueue",id);
            return new ActionCommand(()=>
            {
                 q.Add(cmd);
            });
        }).Execute();
    }
}
