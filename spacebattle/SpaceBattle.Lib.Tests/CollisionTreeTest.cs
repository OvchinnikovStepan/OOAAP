using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using Node = System.Collections.Generic.Dictionary<int, object>;

public class BuildCollisionTreeTest
{
    public BuildCollisionTreeTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        var collisionTree=new Node();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register","Game.Collision.Tree",(object[] args) =>{return collisionTree;}).Execute();
    }
}