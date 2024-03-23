namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;
using Hwdtech;
using Moq;

public class ServerThreadTest_SoftStopWrongThreadTest
{
     [Fact]
    public void SoftStopWrongThreadDoesNotOccure()
    {

        var mre = new ManualResetEvent(false);
        var q1 = new BlockingCollection<ICommand>(100);
        var t1 = new ServerThread(q1);
        var q2 = new BlockingCollection<ICommand>(100);
        var t2 = new ServerThread(q2);
        var endcmd = new Mock<ICommand>();
        endcmd.Setup(endcmd=>endcmd.Execute()).Verifiable();

        var wrongss = IoC.Resolve<ICommand>("Server.Commands.SoftStop", t2, ()=>{endcmd.Object.Execute();});
        var ss1 = IoC.Resolve<ICommand>("Server.Commands.SoftStop", t1);
        var ss2 = IoC.Resolve<ICommand>("Server.Commands.SoftStop", t2, ()=>{mre.Set();});
        q1.Add(new ActionCommand(()=> {}));
        q1.Add(wrongss);    

        q2.Add(new ActionCommand(()=> {}));
        q2.Add(new ActionCommand(()=> {Thread.Sleep(3000);}));

        t1.Start();
        t2.Start();
        q1.Add(new ActionCommand(()=> {Thread.Sleep(1000);}));
        q1.Add(ss1);
        q1.Add(new ActionCommand(()=> {}));

        q2.Add(new ActionCommand(()=> {Thread.Sleep(1000);}));
        q2.Add(ss2);
        mre.WaitOne();

        endcmd.Verify(endcmd=>endcmd.Execute(),Times.Never());
        Assert.Empty(q1); 
    }
}
