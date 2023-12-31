using Hwdtech;

namespace SpaceBattle.Lib
{
    public class LongOperationStrategy : IStrategy
    {
        private readonly string cmd_name;
        private readonly IUObject cmd_target;

        public LongOperationStrategy(string name, IUObject target)
        {
            cmd_name = name;
            cmd_target = target;
        }
        public object Run(params object[] args)
        {
            var cmd = IoC.Resolve<ICommand>("Game.Command." + cmd_name, cmd_target);
            var listcmd = new List<ICommand>() { cmd };
            var macroCmd = IoC.Resolve<ICommand>("Game.Command.Macro", cmd);

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
            "Game.Commands.LongMove", (object[] args) =>
            { return macroCmd; }).Execute();

            var startObject = IoC.Resolve<IStartCommand>("Game.ConvertToStartable", cmd_target);

            var repeatCmd = IoC.Resolve<ICommand>("Game.Command.NewStartCommand", startObject);

            return repeatCmd;
        }
    }
}
