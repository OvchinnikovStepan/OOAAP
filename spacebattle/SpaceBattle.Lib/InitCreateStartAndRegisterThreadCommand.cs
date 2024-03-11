using Hwdtech;
using System.Collections.Concurrent;

namespace SpaceBattle.Lib;

public class InitCreateStartRegisterThreadCmd: Hwdtech.ICommand
{
    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.CreateStartThread", (object[] args)=> {
            var q = new BlockingCollection<ICommand>(100);
            var t = new ServerThread(q);
            return IoC.Resolve<int>("Server.Commands.RegisterThread",q,t);
        }).Execute();
    }
}