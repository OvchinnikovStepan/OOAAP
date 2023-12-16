namespace SpaceBattle.Lib;
using Hwdtech;
using N=Dictionary<int,Dictionary<int,Handler>>;
public interface Handler
    {
        object Run(params object[] args);
    }
public class ExceptionHandler : Handler
    {
        public object Run(params object[] args)
        {
            var command = (ICommand)args[0];
            var commandHash=command.GetType().GetHashCode();
            var exception = (Exception)args[1];
            var exceptionHash=exception.GetType().GetHashCode();
            var tree = IoC.Resolve<N>("Exception.tree");

            Dictionary<int, Handler>? subTree;

            if (!tree.TryGetValue(commandHash, out subTree))
                {
                    subTree = IoC.Resolve<Dictionary<int, Handler>>("Exception.Get.NoCommandSubTree");
                }

            Handler? exceptionHandler;

            if (!subTree.TryGetValue(exceptionHash, out exceptionHandler))
                {
                   return IoC.Resolve<Handler>("Exception.Get.NoExcepetionHandler");
                }
                
            return exceptionHandler;
        }
    }
