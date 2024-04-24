using Hwdtech;
namespace SpaceBattle.Lib
{
    public class InterpreterCommand : Hwdtech.ICommand
    {
        private readonly IMessage _customMessage;

        public InterpreterCommand(IMessage message)
        {
            _customMessage = message;
        }
        public void Execute()
        {
            var cmd = IoC.Resolve<Hwdtech.ICommand>("Game.CreateCommand", _customMessage);

            var id = _customMessage.GameID;
            IoC.Resolve<Hwdtech.ICommand>("Game.Queue.Push", id, cmd).Execute();
        }
    }
}
