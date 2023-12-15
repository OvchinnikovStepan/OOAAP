using Hwdtech;

namespace SpaceBattle.Lib;

public class MacroCommand: ICommand
{
    private readonly List<ICommand> _subcommands=new();

    public MacroCommand(string nameOfDependencyReturnWithSubcommandsNames)
    {
        var subcommandNamesArray=IoC.Resolve<string[]>(nameOfDependencyReturnWithSubcommandsNames);
        foreach(var subcommandName in subcommandNamesArray)
        {
            _subcommands.Add(IoC.Resolve<ICommand>(subcommandName));
        }
    }
    public void Execute()
    {
        foreach(var subcommand in _subcommands)
        {
            subcommand.Execute();
        }
    }
}
