using System.IO;
using ForestInhabitants.Generators;

namespace ForestInhabitants
{
    class Program
    {
        static void Main(string[] args)
        {
            var forest = new ForestLoader(new StreamReader("Maze.txt")).Load();
            new ConsoleVisualisator(forest, new GameBot());
        }
    }
}
