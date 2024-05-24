namespace SpaceBattle.Lib;
using System.Diagnostics;
using Hwdtech;
public class GameCommand : ICommand
{
    private readonly CmdSource Source;
    private readonly object Scope;
    private readonly Stopwatch stopwatch;
    public GameCommand(CmdSource Source, object Scope)
    {
        this.Source = Source;
        this.Scope = Scope;
        stopwatch = new Stopwatch();
    }

    public void Execute()
    {
        var quant = (double)IoC.Resolve<object>("GetGameQuant");
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", Scope).Execute();

        while ((quant >= stopwatch.ElapsedMilliseconds) && (!Source.IsEmpty()))
        {
            stopwatch.Start();
            var command = Source.Take();
            try
            {
                command.Execute();
            }
            catch (Exception e)
            {
                IoC.Resolve<Hwdtech.ICommand>("Game.Commands.ExceptionHandler", command, e).Execute();
            }
            finally
            {
                stopwatch.Stop();
            }        
        }
        stopwatch.Reset();
    }
}
