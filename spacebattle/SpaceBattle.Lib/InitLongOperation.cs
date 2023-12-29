using Hwdtech;

namespace SpaceBattle.Lib
{
    public class InitLongOperationStrategy : ICommand
    {
        public void Execute()
        {
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
           "Game.Commands.InitLO", (object[] args) =>
           { return new LongOperationStrategy((string)args[0], (IUObject)args[1]).Run(); }).Execute();
        }

    }
}
