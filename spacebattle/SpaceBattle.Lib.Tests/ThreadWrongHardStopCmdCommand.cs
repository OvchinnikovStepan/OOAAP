namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;
using Hwdtech;
using Moq;

public class ServerThreadTest_HardStopWrongThreadTest
{

     [Fact]
    public void HardStopWrongThreadDoesNotOccure()
    {

        var mre = new ManualResetEvent(false);
        var q1 = new BlockingCollection<ICommand>(100);
        var t1 = new ServerThread(q1);
        var q2 = new BlockingCollection<ICommand>(100);
        var t2 = new ServerThread(q2);
        var endcmd = new Mock<ICommand>();
        endcmd.Setup(endcmd=>endcmd.Execute()).Verifiable();

        var wronghs = IoC.Resolve<ICommand>("Server.Commands.HardStop", t2, ()=>{endcmd.Object.Execute();});
        var hs1 = IoC.Resolve<ICommand>("Server.Commands.HardStop", t1);
        var hs2 = IoC.Resolve<ICommand>("Server.Commands.HardStop", t2, ()=>{mre.Set();});
        q1.Add(new ActionCommand(()=> {}));
        q1.Add(new ActionCommand(()=> {Thread.Sleep(1000);}));
        q1.Add(wronghs);
        q1.Add(hs1);
        q1.Add(new ActionCommand(()=> {}));

        q2.Add(new ActionCommand(()=> {}));
        q2.Add(new ActionCommand(()=> {Thread.Sleep(3000);}));
        q2.Add(hs2);
        t1.Start();
        t2.Start();
        mre.WaitOne();

        endcmd.Verify(endcmd=>endcmd.Execute(),Times.Never());
        Assert.Single(q1); 
    }
}
