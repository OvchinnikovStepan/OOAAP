using Hwdtech;

namespace SpaceBattle.Lib
{
    public class InitFindVariationsCommand : ICommand
    {
        public void Execute()
        {
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObject.FindVariations", (object[] args) =>
            {
                var positionFirst = (List<int>)args[0];
                var positionSecond = (List<int>)args[1];
                var velocityFirst = (List<int>)args[2];
                var velocitySecond = (List<int>)args[3];

                var variation = positionFirst.Select((value, index) => value - positionSecond[index]).Concat(velocityFirst.Select((value, index) => value - velocitySecond[index])).ToList();
                return variation;
            }).Execute();
        }
    }
}
