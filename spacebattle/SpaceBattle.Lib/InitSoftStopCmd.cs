using Hwdtech;

namespace SpaceBattle.Lib;

public class InitSoftStopCmd: Hwdtech.ICommand
{
    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.SoftStop", (object[] args)=> 
        {
            if (args.Count()==2)
            {
                return new SoftStopCommand((ServerThread)args[0],(Action)args[1]);
            }

            return new SoftStopCommand((ServerThread)args[0],()=>{});
        });
    }
}
