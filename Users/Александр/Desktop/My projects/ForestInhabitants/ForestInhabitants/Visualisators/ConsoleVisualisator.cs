using System;
using System.Collections.Generic;
using System.Threading;
using ForestInhabitants.ForestObjects;
using ForestInhabitants.Generators;

namespace ForestInhabitants
{
    public class ConsoleVisualisator : IForestVisualisator
    {
        public Forest Forest { get; private set; }
        public IForestCommandGenerator Generator { get; private set; }
        private Dictionary<ForestObject, Func<ForestObject,string>> dictionaryOfForestObjects = new Dictionary<ForestObject, Func<ForestObject,string>>();
        public ConsoleVisualisator(Forest forest,IForestCommandGenerator generator)
        {
            InitializeDictionary();
            Forest = forest;
            Generator = generator;
            DrawForest(Forest);            
            Forest.InhabitantCreated += AddInhabitantToDictionary;
            Forest.InhabitantDestroyed += RemoveInhabitantFromDictionary;
            Forest.ForestChange += DrawForest;
            Generator.GenerateCommands(Forest);
        }

        private void InitializeDictionary()
        {
            dictionaryOfForestObjects.Add(new Footpath(), forestObject => String.Format("{0} - тропинка", " "));
            dictionaryOfForestObjects.Add(new Bush(), forestObject => String.Format("{0} - заросли", char.ConvertFromUtf32(9632)[0]));
            dictionaryOfForestObjects.Add(new Trap(), forestObject => String.Format("{0} - капкан", '\u2663'));
            dictionaryOfForestObjects.Add(new Life(), forestObject => String.Format("{0} - жизнь", '\u2665'));
        }

        private bool AddInhabitantToDictionary(Inhabitant inhabitant)
        {
            dictionaryOfForestObjects.Add(inhabitant, forestObject => String.Format("{0} - лесной житель {1} ({2} жизни). Цель - {3},{4}", inhabitant.Name[0], inhabitant.Name, inhabitant.Life,inhabitant.Purpose.X,inhabitant.Purpose.Y));
            return true;
        }

        private bool RemoveInhabitantFromDictionary(Inhabitant inhabitant)
        {
            if (inhabitant.Life <= 0)
                dictionaryOfForestObjects[inhabitant] =
               forestObject => String.Format("{0} - лесной житель {1} УМЕР", inhabitant.Name[0], inhabitant.Name);
            else
                dictionaryOfForestObjects[inhabitant] =
               forestObject => String.Format("{0} - лесной житель {1} ДОСТИГ ЦЕЛИ", inhabitant.Name[0], inhabitant.Name);
            DrawForest(Forest);
            Thread.Sleep(1500);
            dictionaryOfForestObjects.Remove(inhabitant);
            return true;
        }

        public void DrawForest(Forest forest)
        {            
            Console.Clear();
            foreach (var rowForestObjects in forest.Map)
            {
                foreach (var forestObject in rowForestObjects)
                    Console.Write(dictionaryOfForestObjects[forestObject].Invoke(forestObject)[0]);
                Console.WriteLine();
            }
            DrawLegend();
        }

        private void DrawLegend()
        {
            Console.WriteLine();
            foreach (var forestObject in dictionaryOfForestObjects.Keys)
                Console.WriteLine(dictionaryOfForestObjects[forestObject].Invoke(forestObject));
            Console.Beep();
        }
    }
}
