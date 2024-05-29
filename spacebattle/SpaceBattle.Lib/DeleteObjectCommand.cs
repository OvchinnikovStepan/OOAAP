namespace SpaceBattle.Lib;
using Hwdtech;

public class DeleteObjectCommand : Hwdtech.ICommand
{
    private readonly int objectId;

    public DeleteObjectCommand(int objectId)
    {
        this.objectId = objectId;
    }
    public void Execute()
    {
        IoC.Resolve<IDictionary<int, IUObject>>("Game.UObject.List").Remove(objectId);
    }
}
