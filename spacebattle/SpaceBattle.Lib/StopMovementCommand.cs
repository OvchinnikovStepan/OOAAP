using Hwdtech;

namespace SpaceBattle.Lib
{
    public class StopCommand : ICommand
    {
        private IStopCommand stop_table;

        public StopCommand(IStopCommand stop_table)
        {
            this.stop_table = stop_table;
        }

        public void Execute()
        {
            stop_table.Properties.ToList().ForEach(prop => IoC.Resolve<ICommand>("Game.Property.Set", stop_table.Target, prop.Key, prop.Value).Execute());
            var movementCommand = IoC.Resolve<ICommand>("Game.Operations.Movement", stop_table.Target);
            IoC.Resolve<ICommand>("IUObject.Property.Set", stop_table.Target, "Movement", movementCommand).Execute();
            IoC.Resolve<IQueue>("Game.Queue.Push").Add(movementCommand);
        }
    }
}
