using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using SpaceBattle.Lib;
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
      [Fact]
    public void BuildCollisionTreeCommand_Command_Works_Correctly()
    {
        var reader =new Mock<CollisionArraysFromFileReader>();
        var filepath ="../CollisionTestFile.txt";
        var arrays = File.ReadAllLines(filepath).Select(line=>line.Split(" ").Select(int.Parse).ToList()).ToList();
        reader.Setup(r=>r.ReadFile()).Returns(arrays);

        var level1Expected=new HashSet<int>(){1,3,5};
        var level2Expected=new HashSet<int>(){1,7,3,5};
        var level3Expected=new HashSet<int>(){1,3,6};
        var level4Expected=new HashSet<int>(){1,2,4,6};
        var level5Expected=new HashSet<int>(){1,2,4};

        var collisionTreeCommand=new BuildCollisionTreeCommand(reader.Object);

        collisionTreeCommand.Execute();

        var collisionTree =IoC.Resolve<Node>("Game.Collision.Tree");

        var level1Derived=collisionTree.Keys;
        var level2Derived=((Node)collisionTree[1]).Keys.Union(((Node)collisionTree[3]).Keys).Union(((Node)collisionTree[5]).Keys);;
        var level3Derived=new HashSet<int>(){};
        var level4Derived=new HashSet<int>(){};
        var level5Derived=new HashSet<int>(){};

        Assert.True(level1Derived.SequenceEqual(level1Expected));
        Assert.True(level2Derived.SequenceEqual(level2Expected));
        Assert.True(level3Derived.SequenceEqual(level3Expected));
        Assert.True(level4Derived.SequenceEqual(level4Expected));
        Assert.True(level5Derived.SequenceEqual(level5Expected));

    }
}