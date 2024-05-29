using Hwdtech;
using Hwdtech.Ioc;
namespace SpaceBattle.Lib.Test;

public class AdapterBuliderTests
{
    public AdapterBuliderTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }
    [Fact]
    public void AdapterBulider_Works_Correctly()
    {
        var adapter = new AdapterBulider(typeof(IUObject), typeof(IMovable));
        var resultString = adapter.Build();
        var expectationString =
        @"public class IMovableAdapter : IMovable {
        private readonly IUObject obj;
    
        public IMovableAdapter(IUObject obj)
        {
         this.obj = obj;
        }
    
    public Vector Position
    {
    
        get
        {
            return IoC.Resolve<Vector>(""Game.Get.Property"", ""Position"", obj);
        }
    
        set
        {
            return IoC.Resolve<ICommand>(""Game.Set.Property"", ""Position"", obj, value).Execute();
        }
    }
    
    public Vector Velocity
    {
    
        get
        {
            return IoC.Resolve<Vector>(""Game.Get.Property"", ""Velocity"", obj);
        }
    
    }
    }";
        Assert.Equal(resultString, expectationString);
    }
}
