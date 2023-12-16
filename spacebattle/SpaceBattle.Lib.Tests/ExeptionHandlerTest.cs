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

        var exp = new Mock<Exception>();
        var exphash=exp.GetType().GetHashCode();
        var cmd = new Mock<ICommand>();
        var cmdhash=cmd.GetType().GetHashCode();

        var dict = new N { { cmdhash, new Dictionary<int, Handler>() { { exphash, handler.Object } } } };

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception.tree", (object[] args) =>{return dict;}).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception.Get.NotFoundCommandSubTree", (object[] args) =>
            { return new Dictionary<int, Handler>() { { new Mock<Exception>().Object.GetType().GetHashCode(),
                noCmdHandler.Object } };}).Execute();
        
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception.Get.NoExcepetionHandler",(object[] args) => 
            { return noExpHandler.Object; }).Execute();
        }

      


    }