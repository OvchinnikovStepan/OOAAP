namespace SpaceBattle.Lib.Tests;

using SpaceBattle.Lib;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class IQueueExample
{
    [Fact]
    public void IQueueExample1()
    {
        var qReal = new Queue<Lib.ICommand>();
        var qMock = new Mock<IQueue>();
        qMock.Setup(q => q.Take()).Returns(() =>
        {
            return qReal.Dequeue();
        });

        var cmd = new Mock<Lib.ICommand>();
        qReal.Enqueue(cmd.Object);

        Assert.Equal(cmd.Object, qMock.Object.Take());
    }

    [Fact]
    public void IQueueExample2()
    {
        var qReal = new Queue<Lib.ICommand>();
        var qMock = new Mock<IQueue>();
        qMock.Setup(q => q.Take()).Returns(() =>
        {
            return qReal.Dequeue();
        });

        qMock.Setup(q => q.Add(It.IsAny<Lib.ICommand>())).Callback(
        (Lib.ICommand cmd) =>
        {
            qReal.Enqueue(cmd);
        });

        var cmd = new Mock<Lib.ICommand>();
        //qReal.Enqueue(cmd.Object);
        qMock.Object.Add(cmd.Object);

        Assert.Equal(cmd.Object, qMock.Object.Take());
    }
}

public class IoCExempleTests
{
    public IoCExempleTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
    }

    [Fact]
    public void IoCExample1()
    {
        var cmd = new Mock<Lib.ICommand>();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "my_first_dependency",
            (object[] args) =>
            {
                return cmd.Object;
            }
        ).Execute();

        // -------------

        var cmdFromIoC = IoC.Resolve<Lib.ICommand>("my_first_dependency");

        Assert.Equal(cmdFromIoC, cmd.Object);
    }

    [Fact]
    public void IoCExample2()
    {
        var qReal = new Queue<Lib.ICommand>();
        var qMock = new Mock<IQueue>();
        qMock.Setup(q => q.Take()).Returns(() =>
        {
            return qReal.Dequeue();
        });
        qMock.Setup(q => q.Add(It.IsAny<Lib.ICommand>())).Callback(
        (Lib.ICommand cmd) =>
        {
            qReal.Enqueue(cmd);
        });

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.Queue",
            (object[] args) =>
            {
                return qMock.Object;
            }
        ).Execute();

        // ---
        var cmd = new Mock<Lib.ICommand>();
        IoC.Resolve<IQueue>("Game.Queue").Add(cmd.Object);

        // ---

        var cmdFromIoC = IoC.Resolve<IQueue>("Game.Queue").Take();

        Assert.Equal(cmd.Object, cmdFromIoC);

    }

    [Fact]
    public void IoCExample3ActionCommand()
    {
        var ac = new ActionCommand(() =>
        {

        });

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.Command.EmptyCommand",
            (object[] args) =>
            {
                return ac;
            }
        ).Execute();

    }
}

public class ActionCommand : Lib.ICommand
{
    private readonly Action _action;
    public ActionCommand(Action action) => _action = action;
    public void Execute()
    {
        _action();
    }
}

/*
1. Начало длительной операции

interface IUObject {
    object getProperty(string key);
    void setProperty(string key, object value);
}

Приказ {
    Таргет // IUObject UObject, Dict
    НачальныеЗначения {"свойство": "значение"}
}

СтартДлитОперация (Приказ) {
    1. Таргету присвоить начальные значения
    2. var cmd = IoC.Resolve<ICommand>("Game.Operations.Movement", Приказ.Таргет)
    3. IoC.Resolve<ICommand>("IUobject.Property.Set", Приказ.Таргет, "Movement", cmd);
    4. Закинуть в очередь

}

2. Завершение длительной операции
СтопДлительнаяОперация(Приказ) {
    1. Из таргета удалить начальные значения
    2. Заменить в таргете Movement на пустую команду
}

*/
