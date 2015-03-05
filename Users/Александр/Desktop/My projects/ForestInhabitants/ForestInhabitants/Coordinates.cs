using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;

namespace ForestInhabitants
{
    public class Coordinates
    {
        public int X;
        public int Y;

        public Coordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Coordinates Add(Coordinates otherCoordinates)
        {
            return new Coordinates(X + otherCoordinates.X, Y + otherCoordinates.Y);
        }
    }
}
