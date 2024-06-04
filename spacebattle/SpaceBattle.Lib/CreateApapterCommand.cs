namespace SpaceBattle.Lib;
using System.Reflection;
using Hwdtech;

public class CreateAdapterCommand : Hwdtech.ICommand
{
    private readonly Type _insideType;
    private readonly Type _interfaceType;

    public CreateAdapterCommand(Type insideType, Type interfaceType)
    {
        _insideType = insideType;
        _interfaceType = interfaceType;
    }

    public void Execute()
    {
        var codeString = IoC.Resolve<string>("Game.Adapter.Code", _insideType, _interfaceType);
        var assembly = IoC.Resolve<Assembly>("Compile", codeString, _interfaceType);

        var assemblyList = IoC.Resolve<IDictionary<KeyValuePair<Type, Type>, Assembly>>("Game.Adapter.Assembly.List");
        var key = new KeyValuePair<Type, Type>(_insideType, _interfaceType);

        assemblyList[key] = assembly;
    }
}
