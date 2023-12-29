using Hwdtech;

namespace SpaceBattle.Lib
{
    public class StartableEx : IStartCommand
    {
        public IUObject Target { get; }
        public IDictionary<string, object> Properties { get; }
        public StartableEx(IUObject Target, IDictionary<string, object> Properties)
        {
            this.Target = Target;
            this.Properties = Properties;
        }
    }
}
