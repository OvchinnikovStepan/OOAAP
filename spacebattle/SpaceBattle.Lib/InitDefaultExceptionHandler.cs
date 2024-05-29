namespace SpaceBattle.Lib;
using Hwdtech;
public class InitDefaultExceptionHandler : Hwdtech.ICommand
{
    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.ExceptionHandler", (object[] args) =>
        {
            if (!(bool)IoC.Resolve<object>("TryFindStrategy", args[0], args[1]))
            {
                return IoC.Resolve<Hwdtech.ICommand>("DefaultStrategy", args[0], args[1]);
            }

            return IoC.Resolve<Hwdtech.ICommand>("ExeptionStrategy", args[0], args[1]);
        }).Execute();
    }
}
