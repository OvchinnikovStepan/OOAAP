﻿using Hwdtech;

namespace SpaceBattle.Lib;

public class GetObjectStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        return IoC.Resolve<IDictionary<int, IUObject>>("Game.IUObject.List")[(int)args[0]];
    }
}