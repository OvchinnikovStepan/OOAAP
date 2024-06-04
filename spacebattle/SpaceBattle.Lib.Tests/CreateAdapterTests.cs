using System.Reflection;
using Hwdtech;
using Hwdtech.Ioc;
namespace SpaceBattle.Lib.Test;
using Microsoft.CodeAnalysis;
using Moq;

public class CreatingAdapterTests
{
    public CreatingAdapterTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Adapter.Get", (object[] args) =>
        {
            var assemblyList = IoC.Resolve<IDictionary<KeyValuePair<Type, Type>, Assembly>>("Game.Adapter.Assembly.List");
            var pair = new KeyValuePair<Type, Type>((Type)args[0], (Type)args[1]);
            if (!assemblyList.Keys.Contains(pair))
            {
                new CreateAdapterCommand((Type)args[0], (Type)args[1]).Execute();
            }

            return assemblyList[pair];
        }).Execute();
    }
    [Fact]
    public void CreateAdapter_Test()
    {
        var obj = new Mock<IUObject>();
        var metadataReferences = new MetadataReference[]
         {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("SpaceBattle.Lib").Location)
         };
        var assemblyList = new Dictionary<KeyValuePair<Type, Type>, Assembly>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Adapter.Code", (object[] args) =>
    @"namespace SpaceBattle.Lib;
        public class IMovableAdapter : IMovable
        {
            public IMovableAdapter() {}
            public Vector Position
            {
                get => new Vector(new int[] { 0, 1 });
                set => new Vector(new int[] { 0, 1 });
            }
            public Vector Velocity
            {
                get => new Vector(new int[] { 0, 1 });
            }
        }
    ").Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Compile", (object[] args) => new CompileStrategy().Run(args)).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Adapter.Assembly.List", (object[] args) => assemblyList).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Compile.References", (object[] args) => metadataReferences).Execute();

        var assembly = IoC.Resolve<Assembly>("Game.Adapter.Get", typeof(IUObject), typeof(IMovable));

        Assert.Equal("SpaceBattle.Lib.IMovableAdapter", assembly.GetName().Name);
        assembly.GetType("SpaceBattle.Lib.IMovableAdapter");
        var o = Activator.CreateInstance(assembly.GetType("SpaceBattle.Lib.IMovableAdapter")!)!;
        Assert.Equal("SpaceBattle.Lib.IMovableAdapter", o.GetType().ToString());
    }
}
