namespace SpaceBattle.Lib;
using Hwdtech;

public class CreateIUObjectListCommand : Hwdtech.ICommand
{
    private readonly int number_of_objects;

    public CreateIUObjectListCommand(int number_of_objects) { this.number_of_objects = number_of_objects; }

    public void Execute()
    {
        var gameIUObjectList = IoC.Resolve<IDictionary<int, IUObject>>("Game.IUObject.List");

        Enumerable.Range(0, number_of_objects).ToList().ForEach(
            i => gameIUObjectList.Add(i, IoC.Resolve<IUObject>("Game.IUObject.Create"))
        );
    }
}
