namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;
using Hwdtech;
public class ServerThreadTest_SoftStop
{
     [Fact]
    public void SoftStopCommandShouldStopServer()
    {

        var mre = new ManualResetEvent(false);
        var q = new BlockingCollection<ICommand>(100);
        var t = new ServerThread(q);

        var ss = new SoftStopCommand(t, ()=>{mre.Set();});
        q.Add(new ActionCommand(()=> {}));
        q.Add(new ActionCommand(()=> {Thread.Sleep(3000);}));
        q.Add(ss);
        q.Add(new ActionCommand(()=> {}));

        t.Start();
        mre.WaitOne();

        mre.Set();
        Assert.Empty(q); 
    }
}
