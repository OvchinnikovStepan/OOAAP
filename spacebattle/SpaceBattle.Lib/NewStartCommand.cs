using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hwdtech;

namespace SpaceBattle.Lib
{
    public class NewStartCommand : ICommand
    {
        private ICommand c_cmd;
        public NewStartCommand(ICommand cmd)
        {
            c_cmd = cmd;
        }
        public void Execute()
        {
            var injectCmd = IoC.Resolve<ICommand>("Game.Command.Inject", c_cmd);
            var repeatCmd = IoC.Resolve<ICommand>("Game.Command.Repeat", injectCmd);

            repeatCmd.Execute();
        }
    }
}
