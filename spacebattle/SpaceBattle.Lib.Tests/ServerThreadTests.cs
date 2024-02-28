namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;
public class ServerThreadTest
{
    public ServerThreadTest() {
        new InitScopeBasedIoCImplementationCommand().Execute();

//        IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root") )).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Server.Commands.HardStop", (object[] args)=> {
            return new ActionCommand(() => {
                new HardStopCommand((ServerThread)args[0]).Execute();
                new ActionCommand((Action)args[1]).Execute();
            });
        }).Execute();
    }

     [Fact]
    public void SoftStopCommandShouldStopServer()
    {

        var mre = new ManualResetEvent(false);
        var q = new BlockingCollection<ICommand>(100);
        var t = new ServerThread(q);

        var ss = new SoftStopCommand(t,q, ()=>{mre.Set();});
        q.Add(IoC.Resolve<ICommand>(
                "Scopes.Current.Set", 
                IoC.Resolve<object>("Scopes.New", 
                    IoC.Resolve<object>("Scopes.Root")
                ))
        );
        q.Add(new ActionCommand(()=> {}));
        q.Add(new ActionCommand(()=> {Thread.Sleep(3000);}));
        q.Add(ss);
        q.Add(new ActionCommand(()=> {}));


        t.Start();
        mre.WaitOne();

        mre.Set();
        Assert.Empty(q); 
    }


    // [Fact]
    // public void HardStopCommandShouldStopServer()
    // {

    //     var mre = new ManualResetEvent(false);
    //     var q = new BlockingCollection<ICommand>(100);
    //     var t = new ServerThread(q);

    //     var hs = IoC.Resolve<ICommand>("Server.Commands.HardStop", t, ()=>{mre.Set();});

    //     q.Add(new ActionCommand(()=> {}));
    //     q.Add(new ActionCommand(()=> {Thread.Sleep(3000);}));
    //     q.Add(hs);
    //     q.Add(new ActionCommand(()=> {}));


    //     t.Start();
    //     mre.WaitOne();

        
    //     Assert.Single(q); 
    // }

    // [Fact]
    // public void AnExceptionSholdNotStopServerThread()
    // {
    //     var mre = new ManualResetEvent(false);
    //     var q = new BlockingCollection<ICommand>(100);
    //     var t = new ServerThread(q);
    //     var usualCommand = new Mock<ICommand>();
    //     usualCommand.Setup(m => m.Execute()).Verifiable();

    //     var exceptionCommand = new Mock<ICommand>();
    //     exceptionCommand.Setup(m=> m.Execute()).Throws<Exception>().Verifiable();

    //     var hardStopCommand = IoC.Resolve<ICommand>("Server.Commands.HardStop", t, ()=> {mre.Set();});

    //     q.Add(IoC.Resolve<ICommand>(
    //             "Scopes.Current.Set", 
    //             IoC.Resolve<object>("Scopes.New", 
    //                 IoC.Resolve<object>("Scopes.Root")
    //             ))
    //     );

    //     var exceptionHandler = new Mock<ICommand>();
    //     exceptionHandler.Setup(m=> m.Execute()).Verifiable();


    //     q.Add (
    //         IoC.Resolve<ICommand>("IoC.Register", 
    //             "ExceptionHandler.Handle", (object[] args) => exceptionHandler.Object
    //     ));
    //     q.Add(usualCommand.Object);
    //     q.Add(exceptionCommand.Object);
    //     q.Add(usualCommand.Object);
    //     q.Add(hardStopCommand);
    //     q.Add(usualCommand.Object);

    //     t.Start();
    //     mre.WaitOne();

    //     exceptionHandler.Verify(m=>m.Execute(), Times.Once());

    //     usualCommand.Verify(m=>m.Execute(), Times.Exactly(2));
    //     Assert.Single(q);

    // }

}
