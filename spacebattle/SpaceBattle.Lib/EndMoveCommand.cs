using Hwdtech;

namespace SpaceBattle.Lib
{
    public class EndMovementCommand
    {
        private readonly IStopCommand _endable;

        public EndMovementCommand(IStopCommand endable) => _endable = endable;

        public void Execute()
        {
            IoC.Resolve<string>("Game.UObject.DeleteProperty", _endable.Target, _endable.Properties);
            var command = _endable.command;
            var emptyCommand = IoC.Resolve<ICommand>("Game.Command.EmptyCommand");
            IoC.Resolve<IStopCommand>("Game.Command.Inject", command, emptyCommand);
        }
    }
}
