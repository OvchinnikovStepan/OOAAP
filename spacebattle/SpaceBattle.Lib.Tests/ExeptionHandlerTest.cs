namespace SpaceBattle.Lib;

using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using N=Dictionary<int,Dictionary<int,Handler>>;

public class ExceptionHandlerTest
    {
        public ExceptionHandlerTest()
        {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        var handler = new Mock<Handler>();
        var noCmdHandler = new Mock<Handler>();
        var noExpHandler = new Mock<Handler>();

        handler.Setup(x => x.Run()).Returns("SomeHandler");
        noCmdHandler.Setup(x => x.Run()).Returns("No command found Handler"); 
        noExpHandler.Setup(x => x.Run()).Returns("No exeption found Handler");

        var dict = new N() { { new Mock<ICommand>().Object.GetType().GetHashCode(), new Dictionary<int, Handler>()
                 { { new Mock<Exception>().Object.GetType().GetHashCode(), handler.Object } } } };

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception.tree", (object[] args) =>{return dict;}).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception.Get.NoCommandSubTree", (object[] args) =>
            { return new Dictionary<int, Handler>() { { new Mock<Exception>().Object.GetType().GetHashCode(),
                noCmdHandler.Object } };}).Execute();
        
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception.Get.NoExcepetionHandler",(object[] args) => 
            { return noExpHandler.Object; }).Execute();
        }

        [Fact]
         public void ExceptionHandler_Founds_Handler_Correctly()
        {
        var mockCommand = new Mock<SpaceBattle.Lib.ICommand>();
        var mockException = new Mock<System.Exception>();

        var strategy = (Handler)new ExceptionHandler().Run(mockCommand.Object, mockException.Object);
        Assert.Equal("SomeHandler", strategy.Run());
        }

        [Fact]
         public void ExceptionHandler_No_Command_Found_Case_Correct()
         {
        var emptyCommand = new EmptyCommand();
        var exception = new Mock<Exception>();

        var handler = (Handler)new ExceptionHandler().Run(emptyCommand, exception.Object);

        Assert.Equal("No command found Handler", handler.Run());
        }
        [Fact]
        public void ExceptionHandler_No_Exception_Found_Case_Correct()
        {
        var mockCommand = new Mock<ICommand>();
        var exception = new Exception();

        var handler = (Handler)new ExceptionHandler().Run(mockCommand.Object, exception);

        Assert.Equal("No exeption found Handler", handler.Run());
        }
    }
