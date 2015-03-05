using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ForestInhabitants.ForestObjects;
using ForestInhabitants.Generators;

namespace ForestInhabitants
{
    public class ConsoleVisualisator : IForestVisualisator
    {
        public Forest Forest { get; private set; }
        public IForestCommandGenerator Generator { get; private set; }
        private Dictionary<char, string> symbols = new Dictionary<char, string>();
        public ConsoleVisualisator(Forest forest,IForestCommandGenerator generator)
        {
            symbols.Add(new Bush().ToChar(), "заросли");
            symbols.Add(new Trap().ToChar(), "капкан");
            symbols.Add(new Life().ToChar(), "жизнь");
            Forest = forest;
            Generator = generator;
            DrawForest(Forest);
            Forest.ForestChange += DrawForest;
            Generator.GenerateCommands(Forest);
        }
        public void DrawForest(Forest forest)
        {
            Console.Beep();
            Console.Clear();
            for (var i = 0; i < forest.Map.Count; i++)
            {
                for (var j = 0; j < forest.Map[i].Count; j++)
                    Console.Write(VisualisateForestObject(forest.Map[i][j]));
                Console.WriteLine();
            }
            DrawLegend();
        }

        public object VisualisateForestObject(ForestObject forestObject)
        {
            return forestObject.ToChar();
        }

        private void DrawLegend()
        {
            Console.WriteLine();
            foreach (var symbol in symbols.Keys)
                Console.WriteLine("{0} - {1}", symbol, symbols[symbol]);
            foreach (var inhabitant in Forest.Inhabitants)
                Console.WriteLine("{0} - лесной житель {1} ({2} жизни)", inhabitant.ToChar(), inhabitant.Name, inhabitant.Life);
        }
    }
}
