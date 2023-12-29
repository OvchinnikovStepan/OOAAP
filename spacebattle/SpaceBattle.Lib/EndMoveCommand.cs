namespace SpaceBattle.Lib
{
    using Hwdtech;

    public interface IEndable
    {
        public InjectCommand command { get; }
        public IUObject target { get; }
        public IEnumerable<string> property { get; }
    }

    public class EndMovementCommand : ICommand
    {
        private readonly IEndable allow_end;

        public EndMovementCommand(IEndable allow_end)
        {
            this.allow_end = allow_end;
        }

        public void Execute()
        {
            IoC.Resolve<string>("Game.UObject.DeleteProperty", allow_end.target, allow_end.property);
            var command = allow_end.command;
            var emptyCommand = new EmptyCommand();
            IoC.Resolve<IInjectable>("Game.Command.Inject", command, emptyCommand);
        }
    }
}
