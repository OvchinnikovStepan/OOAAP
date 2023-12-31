using Hwdtech;

namespace SpaceBattle.Lib
{
    public class InitConvertToStartable : ICommand
    {
        public void Execute()
        {
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.ConvertToStartable", (object[] args) =>
             {
                 var emptyStartableObject = new StartableEx((IUObject)args[0], new Dictionary<string, object>());
                 return emptyStartableObject;
             }).Execute();
        }
    }
}
