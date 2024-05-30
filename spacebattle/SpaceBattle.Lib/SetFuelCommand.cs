namespace SpaceBattle.Lib;
using Hwdtech;

public class SetFuelCommand : Hwdtech.ICommand
{
    private readonly IEnumerable<IUObject> spaceships;
    private readonly double fuelQuantity;

    public SetFuelCommand(IEnumerable<IUObject> spaceships, double fuelQuantity)
    {
        this.spaceships = spaceships;
        this.fuelQuantity = fuelQuantity;
    }

    public void Execute()
    {
        spaceships.ToList().ForEach(spaceship =>
        IoC.Resolve<Hwdtech.ICommand>("Game.UObject.SetProperty",
         spaceship, "FuelQuantity", fuelQuantity).Execute());
    }
}
