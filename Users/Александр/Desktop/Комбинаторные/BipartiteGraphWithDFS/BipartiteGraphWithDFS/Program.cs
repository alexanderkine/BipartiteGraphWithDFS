using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BipartiteGraphWithDFS
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputFileName = "input.txt";
            var outputFileName = "output.txt";
            if (!File.Exists(inputFileName) || !File.Exists(outputFileName))
                throw new FileNotFoundException();
            var graph = GraphLoader.LoadGraph(new StreamReader(inputFileName));
            var writer = new StreamWriter(outputFileName);
            if (!graph.IsBipartiteGraph())
                writer.Write("N");
            else
            {
                writer.Write("Y\n");
                writer.Write("1 ");
                for (var i = 0; i < graph.ListOfContiguity.Count; i++)
                    if (graph.ListOfEvenTops[i])
                        writer.Write((i + 1) + " ");
                writer.Write("0");
                writer.Write("\n2 ");
                for (var i = 0; i < graph.ListOfContiguity.Count; i++)
                    if (!graph.ListOfEvenTops[i])
                        writer.Write((i + 1)+" ");
                writer.Write("0");
            }
            writer.Close();
        }
    }
}
