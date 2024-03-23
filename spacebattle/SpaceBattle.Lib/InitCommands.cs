namespace SpaceBattle.Lib;
using Hwdtech;
public class InitCommand : ICommand
{
    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.StartServerCommand", (object[] args) =>
        {
            var NumberOfThreads = (int)args[0];

            return new ActionCommand(() =>
            {
                for (var i = 0; i < NumberOfThreads; i++)
                {
                    IoC.Resolve<ICommand>("Game.Commands.CreateAndStartThread").Execute();
                }
            });
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.ExeptionHandler", (object[] args) =>
       {
           var errorFile = IoC.Resolve<string>("GetLogFilePath");
           return new ActionCommand(() =>
           {
               using (var sw = File.AppendText(errorFile))
               {
                   sw.WriteLine("Error occured");
               }
           });
       }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.StopServerCommand", (object[] args) =>
        {
            var ThreadList = IoC.Resolve<List<int>>("Game.Commands.GetThreadIDs");
            return new ActionCommand(() =>
            {
                var barrier = new Barrier(ThreadList.Count() + 1);
                var task = new List<Task>();
                foreach (var i in ThreadList)
                {
                    var stopcmd = IoC.Resolve<ICommand>("Game.Commands.SoftStopThread", i);
                    var expcmd = IoC.Resolve<ICommand>("Game.Commands.ExeptionHandler", stopcmd);
                    task.Add(Task.Run(() =>
                    {
                        try
                        {
                            stopcmd.Execute();
                            barrier.SignalAndWait();
                        }
                        catch
                        {
                            expcmd.Execute();
                            barrier.SignalAndWait();
                        }

                    }));
                }

                barrier.SignalAndWait();
                Task.WaitAll(task.ToArray());
            });
        }).Execute();
    }
}
