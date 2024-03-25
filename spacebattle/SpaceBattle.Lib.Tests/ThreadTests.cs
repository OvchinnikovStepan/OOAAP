namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class ServerThreadTest_Exeption
{
    public ServerThreadTest_Exeption()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        new InitHardStopCmd().Execute();
        new InitSoftStopCmd().Execute();

        new InitCreateStartRegisterThreadCmd().Execute();
        new InitSendCommandCmd().Execute();
        var pill = new ActionCommand(() =>
        {
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
            new InitHardStopCmd().Execute();
        });
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "pill", (object[] args) => { return pill; }).Execute();
        var queueCollection = new Dictionary<int, BlockingCollection<Hwdtech.ICommand>>();
        var threadCollection = new Dictionary<int, ServerThread>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.RegisterThread", (object[] args) =>
        {
            return new ActionCommand(() =>
            {
                queueCollection.Add((int)args[0], (BlockingCollection<Hwdtech.ICommand>)args[1]);
                threadCollection.Add((int)args[0], (ServerThread)args[2]);
            });
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Command.GetThreadQueue", (object[] args) =>
        {
            return queueCollection[(int)args[0]];
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetQueueCollection", (object[] args) => { return queueCollection; }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetThreadCollection", (object[] args) => { return threadCollection; }).Execute();
    }

    [Fact]
    public void AnExceptionSholdNotStopServerThread()
    {
        var mre = new ManualResetEvent(false);
        var q = new BlockingCollection<Hwdtech.ICommand>(100);
        var t = new ServerThread(q);
        var usualCommand = new Mock<Hwdtech.ICommand>();
        usualCommand.Setup(m => m.Execute()).Verifiable();

        var exceptionCommand = new Mock<Hwdtech.ICommand>();
        exceptionCommand.Setup(m => m.Execute()).Throws<Exception>().Verifiable();

        var hardStopCommand = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.HardStop", t, () => { mre.Set(); });

        q.Add(IoC.Resolve<Hwdtech.ICommand>("pill"));

        var exceptionHandler = new Mock<Hwdtech.ICommand>();
        exceptionHandler.Setup(m => m.Execute()).Verifiable();

        q.Add(
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
                "ExceptionHandler.Handle", (object[] args) => exceptionHandler.Object
        ));
        q.Add(usualCommand.Object);
        q.Add(exceptionCommand.Object);
        q.Add(usualCommand.Object);
        q.Add(hardStopCommand);
        q.Add(usualCommand.Object);

        t.Start();
        mre.WaitOne();

        exceptionHandler.Verify(m => m.Execute(), Times.Once());

        usualCommand.Verify(m => m.Execute(), Times.Exactly(2));
        Assert.Single(q);
    }

    [Fact]
    public void SendCommandTest()
    {

        Assert.Empty(IoC.Resolve<Dictionary<int, BlockingCollection<Hwdtech.ICommand>>>("GetQueueCollection"));

        var mre = new ManualResetEvent(false);
        var cmd = new Mock<Hwdtech.ICommand>();
        cmd.Setup(cmd => cmd.Execute()).Verifiable();
        var startcmd = new Mock<Hwdtech.ICommand>();
        startcmd.Setup(cmd => cmd.Execute()).Verifiable();

        IoC.Resolve<Hwdtech.ICommand>("Server.Commands.CreateStartThread", 1, () => { startcmd.Object.Execute(); }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("Server.Commands.CreateStartThread", 2).Execute();

        Assert.True(IoC.Resolve<Dictionary<int, BlockingCollection<Hwdtech.ICommand>>>("GetQueueCollection").Count() == 2);

        IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SendCommand", 1, cmd.Object).Execute();
        IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SendCommand", 2, cmd.Object).Execute();

        var _threadCollection = IoC.Resolve<Dictionary<int, ServerThread>>("GetThreadCollection");

        var hardStopCommand1 = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.HardStop", _threadCollection[1], () => { cmd.Object.Execute(); });
        var hardStopCommand2 = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.HardStop", _threadCollection[2], () =>
        {
            cmd.Object.Execute();
            mre.Set();
        });

        IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SendCommand", 1, hardStopCommand1).Execute();

        IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SendCommand", 2, new ActionCommand(() => { Thread.Sleep(1000); })).Execute();
        IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SendCommand", 2, hardStopCommand2).Execute();

        mre.WaitOne();

        startcmd.Verify(cmd => cmd.Execute(), Times.Once());
        cmd.Verify(cmd => cmd.Execute(), Times.Exactly(4));
    }

    [Fact]
    public void HardStopCommandShouldStopServer()
    {

        var mre = new ManualResetEvent(false);
        var q = new BlockingCollection<Hwdtech.ICommand>(100);
        var t = new ServerThread(q);
        t.GetHashCode();
        var hs = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.HardStop", t, () => { mre.Set(); });

        q.Add(new ActionCommand(() => { }));
        q.Add(new ActionCommand(() => { Thread.Sleep(3000); }));
        q.Add(hs);
        q.Add(new ActionCommand(() => { }));

        t.Start();
        mre.WaitOne();

        Assert.Single(q);
    }
    [Fact]
    public void HardStopWrongThreadDoesNotOccure()
    {

        var mre = new ManualResetEvent(false);
        var q1 = new BlockingCollection<Hwdtech.ICommand>(100);
        var t1 = new ServerThread(q1);
        var q2 = new BlockingCollection<Hwdtech.ICommand>(100);
        var t2 = new ServerThread(q2);
        var endcmd = new Mock<Hwdtech.ICommand>();
        endcmd.Setup(endcmd => endcmd.Execute()).Verifiable();

        var wronghs = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.HardStop", t2, () => { endcmd.Object.Execute(); });
        var hs1 = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.HardStop", t1);
        var hs2 = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.HardStop", t2, () => { mre.Set(); });
        q1.Add(new ActionCommand(() => { }));
        q1.Add(new ActionCommand(() => { Thread.Sleep(1000); }));
        q1.Add(wronghs);
        q1.Add(hs1);
        q1.Add(new ActionCommand(() => { }));

        q2.Add(new ActionCommand(() => { }));
        q2.Add(new ActionCommand(() => { Thread.Sleep(3000); }));
        q2.Add(hs2);
        t1.Start();
        t2.Start();
        mre.WaitOne();

        endcmd.Verify(endcmd => endcmd.Execute(), Times.Never());
        Assert.Single(q1);
    }
    [Fact]
    public void SoftStopWrongThreadDoesNotOccure()
    {

        var mre = new ManualResetEvent(false);
        var q1 = new BlockingCollection<Hwdtech.ICommand>(100);
        var t1 = new ServerThread(q1);
        var q2 = new BlockingCollection<Hwdtech.ICommand>(100);
        var t2 = new ServerThread(q2);
        var endcmd = new Mock<Hwdtech.ICommand>();
        endcmd.Setup(endcmd => endcmd.Execute()).Verifiable();

        var wrongss = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SoftStop", t2, () => { endcmd.Object.Execute(); });
        var ss1 = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SoftStop", t1);
        var ss2 = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SoftStop", t2, () => { mre.Set(); });
        q1.Add(IoC.Resolve<Hwdtech.ICommand>("pill"));
        q1.Add(wrongss);

        q2.Add(IoC.Resolve<Hwdtech.ICommand>("pill"));
        q2.Add(new ActionCommand(() => { Thread.Sleep(3000); }));

        t1.Start();
        t2.Start();
        q1.Add(new ActionCommand(() => { Thread.Sleep(1000); }));
        q1.Add(ss1);
        q1.Add(new ActionCommand(() => { }));

        q2.Add(new ActionCommand(() => { Thread.Sleep(1000); }));
        q2.Add(ss2);
        mre.WaitOne();

        endcmd.Verify(endcmd => endcmd.Execute(), Times.Never());
        Assert.Empty(q1);
    }
    [Fact]
    public void SoftStopCommandShouldStopServer()
    {

        var mre = new ManualResetEvent(false);
        var q = new BlockingCollection<Hwdtech.ICommand>(100);
        var t = new ServerThread(q);

        var ss = new SoftStopCommand(t, () => { mre.Set(); });
        q.Add(IoC.Resolve<Hwdtech.ICommand>("pill"));
        q.Add(ss);
        q.Add(new ActionCommand(() => { }));

        t.Start();
        mre.WaitOne();

        Assert.Empty(q);
    }
    [Fact]
    public void EqualsTest()
    {
        var q = new BlockingCollection<Hwdtech.ICommand>(100);
        var t = new ServerThread(q);

        t.Equals(null);
    }
}
