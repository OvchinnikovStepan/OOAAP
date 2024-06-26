﻿namespace SpaceBattle.Lib;

public class ActionCommand : Hwdtech.ICommand
{
    private readonly Action _action;
    public ActionCommand(Action action) => _action = action;
    public void Execute()
    {
        _action();
    }
}
