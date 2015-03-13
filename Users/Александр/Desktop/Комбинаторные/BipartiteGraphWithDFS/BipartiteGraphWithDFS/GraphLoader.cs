using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BipartiteGraphWithDFS
{
    public static class GraphLoader
    {
        public static Graph LoadGraph(StreamReader inputStream)
        {
            var splitString = inputStream.ReadToEnd().Split(new[] {'\n', '\r','0'}, StringSplitOptions.RemoveEmptyEntries);
            var graph = new Graph { ListOfContiguity = new List<List<int>>()};
            for (var i = 1; i <= int.Parse(splitString[0]); i++)
            {
                graph.ListOfContiguity.Add(new List<int>());
                var tops = splitString[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (var j = 0; j < tops.Length; j++)
                    graph.ListOfContiguity[i-1].Add(int.Parse(tops[j])-1);
            }
            inputStream.Close();
            return graph;
        }
    }
}
