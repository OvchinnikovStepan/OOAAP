using Hwdtech;
using System.Collections.Concurrent;

namespace SpaceBattle.Lib;

public class InitSendCommandCmd: Hwdtech.ICommand
{
    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.SendCommand", (object[] args)=> {
            var q=IoC.Resolve<BlockingCollection<Hwdtech.ICommand>>("Server.Command.GetThreadQueue",args[0]);
            q.Add((Hwdtech.ICommand)args[1]);
        }).Execute();
    }
}