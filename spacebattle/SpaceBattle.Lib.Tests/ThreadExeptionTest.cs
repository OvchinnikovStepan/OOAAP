namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class ServerThreadTest_Exeption
{
    public ServerThreadTest_Exeption() {
        new InitScopeBasedIoCImplementationCommand().Execute();
        new InitHardStopCmd().Execute();
        new InitSoftStopCmd().Execute();
        IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root") )).Execute();

         var queueCollection = new Dictionary<int,BlockingCollection<ICommand>>();
        var threadCollection = new Dictionary<int,ServerThread>();
        IoC.Resolve<ICommand>("IoC.Register","Server.Commands.RegisterThread",(object[] args)=>
        {
            return new ActionCommand(()=>
            {
                 queueCollection.Add((int)args[0],(BlockingCollection<ICommand>)args[1]);
                 threadCollection.Add((int)args[0],(ServerThread)args[2]);
            });
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register","Server.Command.GetThreadQueue",(object[]args)=>
        {
            return queueCollection[(int)args[0]];
        }).Execute();
        new InitCreateStartRegisterThreadCmd().Execute();
        new InitSendCommandCmd().Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register","GetQueueCollection",(object[] args)=>{return queueCollection;}).Execute();
    }

    [Fact]
    public void AnExceptionSholdNotStopServerThread()
    {
        var mre = new ManualResetEvent(false);
        var q = new BlockingCollection<ICommand>(100);
        var t = new ServerThread(q);
        var usualCommand = new Mock<ICommand>();
        usualCommand.Setup(m => m.Execute()).Verifiable();

        var exceptionCommand = new Mock<ICommand>();
        exceptionCommand.Setup(m=> m.Execute()).Throws<Exception>().Verifiable();

        var hardStopCommand = IoC.Resolve<ICommand>("Server.Commands.HardStop", t, ()=> {mre.Set();});

        q.Add(IoC.Resolve<ICommand>(
                "Scopes.Current.Set", 
                IoC.Resolve<object>("Scopes.New", 
                    IoC.Resolve<object>("Scopes.Root")
                ))
        );

        var exceptionHandler = new Mock<ICommand>();
        exceptionHandler.Setup(m=> m.Execute()).Verifiable();

        q.Add (
            IoC.Resolve<ICommand>("IoC.Register", 
                "ExceptionHandler.Handle", (object[] args) => exceptionHandler.Object
        ));
        q.Add(usualCommand.Object);
        q.Add(exceptionCommand.Object);
        q.Add(usualCommand.Object);
        q.Add(hardStopCommand);
        q.Add(usualCommand.Object);

        t.Start();
        mre.WaitOne();

        exceptionHandler.Verify(m=>m.Execute(), Times.Once());

        usualCommand.Verify(m=>m.Execute(), Times.Exactly(2));
        Assert.Single(q);
    }

     [Fact]
    public void SendCommandTest()
    {
        Assert.Empty(IoC.Resolve<Dictionary<int,BlockingCollection<ICommand>>>("GetQueueCollection"));

        var cmd = new Mock<ICommand>();
        cmd.Setup(cmd=>cmd.Execute()).Verifiable();
        var startcmd = new Mock<ICommand>();
        startcmd.Setup(cmd=>cmd.Execute()).Verifiable();

        IoC.Resolve<ICommand>("Server.Commands.CreateStartThread",1,()=>{startcmd.Object.Execute();}).Execute();
        IoC.Resolve<ICommand>("Server.Commands.CreateStartThread",2).Execute();
  

        Assert.True(IoC.Resolve<Dictionary<int,BlockingCollection<ICommand>>>("GetQueueCollection").Count()==2);
       
        IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SendCommand",1,cmd.Object).Execute();
        IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SendCommand",2,cmd.Object).Execute();

        startcmd.Verify(cmd=>cmd.Execute(),Times.Once());
        cmd.Verify(cmd=>cmd.Execute(),Times.Exactly(2));
    }


}
