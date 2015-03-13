using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BipartiteGraphWithDFS
{
    public class Graph
    {
        public List<List<int>> ListOfContiguity;
        public List<bool> ListOfEvenTops;
        public bool IsBipartiteGraph()
        {
            var topsStack = new Stack<int>();
            var visitedTops = Enumerable.Range(0, ListOfContiguity.Count).Select(x => false).ToList();
            ListOfEvenTops = Enumerable.Range(0, ListOfContiguity.Count).Select(x => true).ToList();
            topsStack.Push(0);
            visitedTops[0] = true;
            while (topsStack.Count != 0)
            {
                var top = topsStack.Pop();
                foreach (var neighbour in ListOfContiguity[top].Where(x => !visitedTops[x]))
                {            
                    if (!visitedTops[neighbour])
                    {
                        ListOfEvenTops[neighbour] = !ListOfEvenTops[top];
                        topsStack.Push(neighbour);
                        visitedTops[neighbour] = true;
                        continue;
                    }
                    if (ListOfEvenTops[neighbour] == ListOfEvenTops[top])
                        return false;
                }
            }
            return true;
        }
    }
}
