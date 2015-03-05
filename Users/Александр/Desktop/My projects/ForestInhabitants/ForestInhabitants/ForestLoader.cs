using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForestInhabitants.ForestObjects;

namespace ForestInhabitants
{
    public class ForestLoader
    {
        private readonly StreamReader reader;
        public ForestLoader(StreamReader reader)
        {
            if (reader == null)
                throw new NullReferenceException();
            this.reader = reader;
        }
        private Dictionary<char, ForestObject> convertionDictionary = new Dictionary<char, ForestObject>();

        public Forest Load()
        {
            FillConvertionDictionary();
            var forest = new Forest();
            var map = reader.ReadToEnd().Split(new[] {'\n','\r'}, StringSplitOptions.RemoveEmptyEntries);
            reader.Close();
            for (var i = 0; i < map.Length; i++)
            {
                forest.Map.Add(new List<ForestObject>());
                for (var j = 0; j < map[i].Length; j++)
                {
                    if (!convertionDictionary.ContainsKey(map[i][j]))
                        map[i] = map[i].Replace(map[i][j], convertionDictionary.Keys.ElementAt(0));
                    forest.Map[i].Add(convertionDictionary[map[i][j]].CoordinateObject(new Coordinates(j, i)));
                }
            }
            return forest;
        }

        private void FillConvertionDictionary()
        {
            convertionDictionary.Add('0', new Footpath());
            convertionDictionary.Add('1', new Bush());
            convertionDictionary.Add('K', new Trap());
            convertionDictionary.Add('L', new Life());
        }
    }
}
