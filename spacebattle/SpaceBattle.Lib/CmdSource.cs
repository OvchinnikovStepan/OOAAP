namespace SpaceBattle.Lib;

public interface CmdSource
{
    Hwdtech.ICommand Take();
    bool IsEmpty();
}
