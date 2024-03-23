using Hwdtech;
using System.Collections.Concurrent;

namespace SpaceBattle.Lib;

public class InitCreateStartRegisterThreadCmd: Hwdtech.ICommand
{
    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.CreateStartThread", (object[] args)=> {
           var id=(int)args[0];
           Action action;
           if (args.Count()==1)
           {
             action = (()=>{});
           }
           else
           {
             action=(Action)args[1];
           }
           return new ActionCommand(()=>
           {
            var q = new BlockingCollection<ICommand>(100);
            var t = new ServerThread(q);
            t.Start();
            IoC.Resolve<ICommand>("Server.Commands.RegisterThread",id,q,t).Execute();  
            action();
           });
        }).Execute();
    }
}
