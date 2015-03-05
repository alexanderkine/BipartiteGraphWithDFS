using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForestInhabitants.ForestObjects;
using ForestInhabitants.Generators;
using Ninject.Modules;
using Ninject;

namespace ForestInhabitants
{
    class Program
    {
        static void Main(string[] args)
        {
            var forest = new ForestLoader(new StreamReader("RandomMap.txt")).Load();
            new ConsoleVisualisator(forest, new UserCommandGenerator());
            //var testForest = new ForestLoader(new StreamReader("TestMap.txt")).Load();
            //var testInhabitant = new Inhabitant("Alex", 10, new Coordinates(1, 1));
            //testForest.CreateInhabitant(testInhabitant, testInhabitant.Place);
            //testForest.Move(testInhabitant, MoveCommand.Up);
            //testForest.Move(testInhabitant, MoveCommand.Up);
        }
    }
}
