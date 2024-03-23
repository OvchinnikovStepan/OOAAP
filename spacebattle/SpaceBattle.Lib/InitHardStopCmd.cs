using Hwdtech;

namespace SpaceBattle.Lib;

public class InitHardStopCmd: Hwdtech.ICommand
{
    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.HardStop", (object[] args)=> {
            
            if (args.Count()==2)
            {
                return new ActionCommand(()=>
                {
                    if(((ServerThread)args[0]).Equals(Thread.CurrentThread))
                    {
                       new HardStopCommand((ServerThread)args[0]).Execute();
                       new ActionCommand((Action)args[1]).Execute();
                    }
                 });
            }
            
            return new ActionCommand(() => {
                  if(((ServerThread)args[0]).Equals(Thread.CurrentThread))
                    {
                       new HardStopCommand((ServerThread)args[0]).Execute();
                    }  });
        }).Execute();
    }
}
