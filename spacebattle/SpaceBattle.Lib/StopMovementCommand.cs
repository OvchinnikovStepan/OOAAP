using Hwdtech;

namespace SpaceBattle.Lib
{
    public class StopCommand : ICommand
    {
        private IStopCommand stoptable;

        public StopCommand(IStopCommand stoptable)
        {
            this.stoptable = stoptable;
        }

        public void Execute()
        {
            stoptable.Properties.ToList().ForEach(property =>
            {
                var propertySetCommand = IoC.Resolve<ICommand>("Game.Property.Set", stoptable.Target, property.Key, property.Value);
                propertySetCommand.Execute();
            });

            var stopMovementCommand = IoC.Resolve<ICommand>("Game.Operations.StopMovement", stoptable.Target);
            stopMovementCommand.Execute();

            var movementPropertySetCommand = IoC.Resolve<ICommand>("IUObject.Property.Set", stoptable.Target, "Movement", stopMovementCommand);
            movementPropertySetCommand.Execute();

            var queuePushCommand = IoC.Resolve<IQueue>("Game.Queue.Push");
            queuePushCommand.Add(movementPropertySetCommand);
        }
    }
}
