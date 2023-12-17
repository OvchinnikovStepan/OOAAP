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
            var repeatCmd = IoC.Resolve<ICommand>("Game.Command.NewStartCommand", cmd);

            return repeatCmd;
        }
    }
}
