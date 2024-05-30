namespace SpaceBattle.Lib;
using Hwdtech;

public class RegisterCommandsCommand : ICommand
{
    public void Execute()
    {
        var dependencies = IoC.Resolve<IDictionary<string, IStrategy>>("Game.Dependencies.Get.Commands");

        dependencies.ToList().ForEach(dependency =>
        {
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command." + dependency.Key, (object[] args) => dependency.Value.Run(args)).Execute();
        });
    }
}
