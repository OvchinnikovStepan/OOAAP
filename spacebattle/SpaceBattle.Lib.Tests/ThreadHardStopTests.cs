namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;
using Hwdtech;
public class ServerThreadTest_HardStop
{

     [Fact]
    public void HardStopCommandShouldStopServer()
    {

        var mre = new ManualResetEvent(false);
        var q = new BlockingCollection<ICommand>(100);
        var t = new ServerThread(q);

        var hs = IoC.Resolve<ICommand>("Server.Commands.HardStop", t, ()=>{mre.Set();});

        q.Add(new ActionCommand(()=> {}));
        q.Add(new ActionCommand(()=> {Thread.Sleep(3000);}));
        q.Add(hs);
        q.Add(new ActionCommand(()=> {}));

        t.Start();
        mre.WaitOne();

        
        Assert.Single(q); 
    }
}
