namespace SpaceBattle.Lib
{
    public interface IStopCommand
    {
        public InjectCommand command { get; }
        public IUObject Target { get; }
        public IDictionary<string, object> Properties { get; }
    }
}
