using System.Reflection;
using Hwdtech;
using Hwdtech.Ioc;
namespace SpaceBattle.Lib.Test;
using Microsoft.CodeAnalysis;

public class CreatingAdapterTests
{
    public CreatingAdapterTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }
    [Fact]
    public void CreateAdapter_Test()
    {
        var metadataReferences = new MetadataReference[]
         {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("SpaceBattle.Lib").Location)
         };
        var assemblyList = new Dictionary<KeyValuePair<Type, Type>, Assembly>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Adapter.Code", (object[] args) =>
    @"namespace SpaceBattle.Lib{
        public class IMovableAdapter : IMovable
        {
            private IUObject obj;
            public IMovableAdapter(IUObject obj) => this.obj = obj;
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
    }").Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Compile", (object[] args) => new CompileStrategy().Run(args)).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Adapter.Assembly.List", (object[] args) => assemblyList).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Compile.References", (object[] args) => metadataReferences).Execute();
        new CreateAdapterCommand(typeof(IUObject), typeof(IMovable)).Execute();

        Assert.Equal("SpaceBattle.Lib.IMovableAdapter", assemblyList[new KeyValuePair<Type, Type>(typeof(IUObject), typeof(IMovable))].GetName().Name);
    }
}
