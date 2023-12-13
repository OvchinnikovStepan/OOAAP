using Hwdtech;

namespace SpaceBattle.Lib
{

    public class StopCommand : ICommand, IStopCommand
    {
        private IUObject target;
        private IDictionary<string, object> properties;

        public StopCommand(IUObject target, IDictionary<string, object> properties)
        {
            this.target = target;
            this.properties = properties;
        }
        public void Execute()
        {
            if (target != null && properties != null)
            {
                target.SetProperty("IsMoving", false);
                foreach (var property in properties)
                {
                    target.SetProperty(property.Key, property.Value);
                }
            }
        }
    }

    public IUObject Target
    {
        get  return target;
        }

    public IDictionary<string, object> Properties
    {
        get  return properties;
        }
    }
    