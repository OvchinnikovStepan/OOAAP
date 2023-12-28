namespace SpaceBattle.Lib
{
    public interface IInjectable
    {
        void Inject(ICommand obj);
    }

    public class InjectCommand : ICommand, IInjectable
    {
        private ICommand command;

        public InjectCommand(ICommand cmd)
        {
            command = cmd;
        }

        public void Execute()
        {
            command.Execute();
        }

        public void Inject(ICommand obj)
        {
            command = obj;
        }
    }
}