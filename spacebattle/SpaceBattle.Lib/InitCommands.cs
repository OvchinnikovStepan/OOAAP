namespace SpaceBattle.Lib;
using System.Linq;
using Hwdtech;
public class InitCommand : Hwdtech.ICommand
{
    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.StartServerCommand", (object[] args) =>
        {
            var NumberOfThreads = (int)args[0];

            return new ActionCommand(() =>
            {
                Enumerable.Range(0, NumberOfThreads).ToList().ForEach(i =>
                {
                    IoC.Resolve<Hwdtech.ICommand>("Game.Commands.CreateAndStartThread").Execute();
                });
            });
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.ExeptionHandler", (object[] args) =>
       {
           var errorFile = IoC.Resolve<string>("GetLogFilePath");
           return new ActionCommand(() =>
           {
               using (var sw = File.AppendText(errorFile))
               {
                   sw.WriteLine($"Error occurred in command: {args[0]}");
                   sw.WriteLine($"Exception: {args[1]}");
               }
           });
       }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.StopServerCommand", (object[] args) =>
        {
            var ThreadList = IoC.Resolve<List<int>>("Game.Commands.GetThreadIDs");
            return new ActionCommand(() =>
            {
                ThreadList.ForEach(thread_id =>
                 {
                     IoC.Resolve<Hwdtech.ICommand>("Game.Commands.SendCommand", thread_id,
                IoC.Resolve<Hwdtech.ICommand>("Game.Commands.SoftStopThread", thread_id,
                IoC.Resolve<Action>("Game.Commands.StopServerBarrierRemove"))).Execute();
                 });

                IoC.Resolve<Hwdtech.ICommand>("Game.Commands.StopServerBarrierWait").Execute();
            });
        }).Execute();
    }
}
