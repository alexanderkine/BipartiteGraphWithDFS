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
        private Dictionary<ForestObject, string> dictionaryOfForestObjects = new Dictionary<ForestObject, string>();
        public ConsoleVisualisator(Forest forest,IForestCommandGenerator generator)
        {
            InitializeDictionary();
            Forest = forest;
            Generator = generator;
            DrawForest(Forest);            
            Forest.InhabitantCreated += AddInhabitantToDictionary;
            Forest.InhabitantMove += ChangeInhabitantIntoDictionary;
            Forest.InhabitantDestroyed += RemoveInhabitantFromDictionary;
            Forest.ForestChange += DrawForest;
            Generator.GenerateCommands(Forest);
        }

        private void InitializeDictionary()
        {
            dictionaryOfForestObjects.Add(new Footpath(), String.Format("{0} - тропинка", " "));
            dictionaryOfForestObjects.Add(new Bush(), String.Format("{0} - заросли", char.ConvertFromUtf32(9632)[0]));
            dictionaryOfForestObjects.Add(new Trap(), String.Format("{0} - капкан", '\u2663'));
            dictionaryOfForestObjects.Add(new Life(), String.Format("{0} - жизнь", '\u2665'));
        }

        private bool ChangeInhabitantIntoDictionary(Inhabitant inhabitant)
        {
            dictionaryOfForestObjects[inhabitant] = String.Format("{0} - лесной житель {1} ({2} жизни)",
                inhabitant.Name[0], inhabitant.Name, inhabitant.Life);
            return true;
        }               

        private bool AddInhabitantToDictionary(Inhabitant inhabitant)
        {
            dictionaryOfForestObjects.Add(inhabitant,
                String.Format("{0} - лесной житель {1} ({2} жизни)", inhabitant.Name[0], inhabitant.Name, inhabitant.Life));
            return true;
        }

        private bool RemoveInhabitantFromDictionary(Inhabitant inhabitant)
        {
            dictionaryOfForestObjects.Remove(inhabitant);
            return true;
        }

        public void DrawForest(Forest forest)
        {
            Console.Beep();
            Console.Clear();
            foreach (var rowForestObjects in forest.Map)
            {
                foreach (var forestObject in rowForestObjects)
                {
                    var selectKey =
                         dictionaryOfForestObjects.Keys.FirstOrDefault(otherForestObject => forestObject.Equals(otherForestObject)); // ищет первое сопоставление ForestObject по ссылке (примечательно для Inhabitant)
                    selectKey = selectKey ?? dictionaryOfForestObjects.Keys.First(otherForestObject => forestObject.GetType() == otherForestObject.GetType()); // если не нашло, то ищет первое сравнение по типу объектов
                    Console.Write(dictionaryOfForestObjects[selectKey][0]);
                }
                Console.WriteLine();
            }
            DrawLegend();
        }

        private void DrawLegend()
        {
            Console.WriteLine();
            foreach (var forestObject in dictionaryOfForestObjects.Keys)
                Console.WriteLine(dictionaryOfForestObjects[forestObject]);
        }
    }
}
