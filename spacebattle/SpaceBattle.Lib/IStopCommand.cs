namespace SpaceBattle.Lib
{
    public interface IStopCommand
    {
        ICommand command { get; }
        IUObject Target { get; }
        IDictionary<string, object> Properties { get; }
    }
}
