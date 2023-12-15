namespace SpaceBattle.Lib
{
    using Dict = System.Collections.Generic.Dictionary<int, object>;
    using Hwdtech;
    using System.IO;
    using System.Linq;

    public class DecisionTreeCommand : ICommand
    {
        private readonly string file_path;

        public DecisionTreeCommand(string path)
        {
            file_path = path;
        }

        public void Execute()
        {
            var vectors = ReadDataFromFile(file_path);
            var tree = IoC.Resolve<Dict>("Game.DecisionTree");
            CreateDecisionTree(tree, vectors);
        }

        private static List<List<int>> ReadDataFromFile(string path)
        {
            return File.ReadAllLines(path)
                .Select(line => line.Split().Select(int.Parse).ToList())
                .ToList();
        }

        private static void CreateDecisionTree(Dict tree, List<List<int>> vectors)
        {
            vectors.ForEach(vector =>
            {
                var lvl = tree;
                vector.ForEach(num =>
                {
                    AddToTreeLayer(lvl, num);
                    lvl = (Dict)lvl[num];
                });
            });
        }

        private static void AddToTreeLayer(Dict lvl, int num)
        {
            if (!lvl.ContainsKey(num))
            {
                lvl[num] = new Dict();
            }
        }
    }
}

