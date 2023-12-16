using Hwdtech;

namespace SpaceBattle.Lib
{
    public class CollisionCheckCommand : ICommand
    {
        private readonly IUObject obj_1, obj_2;

        public CollisionCheckCommand(IUObject first_obj, IUObject second_obj)
        {
            obj_1 = first_obj;
            obj_2 = second_obj;
        }

        public void Execute()
        {
            var first_position = IoC.Resolve<List<int>>("Game.UObject.GetProperty", obj_1, "Position");
            var first_velocity = IoC.Resolve<List<int>>("Game.UObject.GetProperty", obj_1, "Velocity");
            var second_position = IoC.Resolve<List<int>>("Game.UObject.GetProperty", obj_2, "Position");
            var second_velocity = IoC.Resolve<List<int>>("Game.UObject.GetProperty", obj_2, "Velocity");

            var variations = IoC.Resolve<List<int>>("Game.UObject.FindVariations", first_position, second_position, first_velocity, second_velocity);

            var collisionTree = IoC.Resolve<IDictionary<int, object>>("Game.Command.BuildCollisionTree");

            variations.ForEach(variation => collisionTree = (IDictionary<int, object>)collisionTree[variation]);

            IoC.Resolve<ICommand>("Game.Event.Collision", obj_1, obj_2).Execute();
        }
    }
}
