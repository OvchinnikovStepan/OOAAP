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

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.StopServerCommand", (object[] args) =>
        {
            var ThreadList = IoC.Resolve<List<int>>("Game.Commands.GetThreadIDs");
            return new ActionCommand(() =>
            {
                foreach (var i in ThreadList)
                {
                    try
                    {
                        IoC.Resolve<ICommand>("Game.Commands.SoftStopThread", i).Execute();
                    }
                    catch (Exception e)
                    {
                        // var errorFile = Path.GetTempFileName();
                        // File.WriteAllText(errorFile, e.ToString());
                        IoC.Resolve<ICommand>("Game.Commands.ExeptionLog", IoC.Resolve<string>("GetLogFilePath"), e).Execute();
                    }
                }
            });
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.ExeptionLog", (object[] args) =>
        {
            var errorFile = (string)args[0];
            return new ActionCommand(() =>
            {
                File.WriteAllText(errorFile, "Error occurred");
            });
        }).Execute();
    }
}
