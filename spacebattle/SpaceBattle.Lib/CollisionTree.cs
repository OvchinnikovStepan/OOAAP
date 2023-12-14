using Hwdtech;
using Node=System.Collections.Generic.Dictionary<int,object>;

namespace SpaceBattle.Lib;

public interface CollisionArraysFromFileReader
{
    public List<int[]> ReadFile();
}

public class BuildCollisionTreeCommand: ICommand
{
    private readonly CollisionArraysFromFileReader reader;

    public BuildCollisionTreeCommand(CollisionArraysFromFileReader reader)
    {
        this.reader=reader;
    }

    public void Execute()
    {
        var arrays = reader.ReadFile();
        foreach (var array in arrays)
        {
            var point = IoC.Resolve<Node>("Game.Collision.Tree");
            foreach (var number in array)
            {
                point.TryAdd(number,new Node());
                point=(Node)point[number];
            }
        }
    }
}