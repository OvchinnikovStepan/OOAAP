namespace SpaceBattle.Lib
{
    public interface IStopCommand
    {
        public IUObject Target { get; }
        public IDictionary<string, object> property { get; }
    }
}
