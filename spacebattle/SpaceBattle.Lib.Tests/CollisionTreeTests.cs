namespace SpaceBattle.Lib.Tests;
using Dict = Dictionary<int, object>;
using Hwdtech;
using Hwdtech.Ioc;

public class DecisionTreeTest
{
    readonly string path;

    public DecisionTreeTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
            IoC.Resolve<object>("Scopes.Root")))
        .Execute();

        var tree = new Dict();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.DecisionTree",
            (object[] args) => tree
        ).Execute();

        path = @"../../../Vector.txt";
    }

    [Fact]
    public void BuildDecisionTreeWithoutExceptions()
    {
        var decisionTreeCommand = new DecisionTreeCommand(path);
        decisionTreeCommand.Execute();

        var tree = IoC.Resolve<Dict>("Game.DecisionTree");

        Assert.True(tree.ContainsKey(3));

        Assert.True(((Dict)tree[3]).ContainsKey(4));

        Assert.True(((Dict)((Dict)tree[3])[4]).ContainsKey(6));

        Assert.True(
            ((Dict)((Dict)((Dict)tree[3])[4])[6]).ContainsKey(2)
        );
    }

    [Fact]
    public void DecisionTreeCommandExecuteTest()
    {
        var decisionTreeCommand = new DecisionTreeCommand(path);
        Assert.NotNull(decisionTreeCommand);
    }

    [Fact]
    public void ImpossibleToReadFileWhenBuildDecisionTree()
    {
        var wrongPath = "wrongVectors";
        var decisionTreeCommand = new DecisionTreeCommand(wrongPath);
        Assert.Throws<FileNotFoundException>(decisionTreeCommand.Execute);
    }
}
