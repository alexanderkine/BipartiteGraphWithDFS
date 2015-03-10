using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;

namespace ForestInhabitants.ForestObjects
{
    public class Inhabitant : ForestObject
    {
        public string Name;
        public int Life;
        public ForestObject PrevObject;
        public Coordinates Purpose;
        public Inhabitant(string name, int life)
        {
            Name = name;
            Life = life;
        }

        public override bool CanMove { get { return false; }  }

        public override bool CanEnter(ref Inhabitant inhabitant, ref List<List<ForestObject>> map, Coordinates place)
        {
            return false;
        }

        public bool Leave(ref List<List<ForestObject>> map, ForestObject prevObject)
        {
            map[prevObject.Place.Y][prevObject.Place.X] = prevObject;
            return true;
        }

        public override ForestObject CoordinateObject(Coordinates coordinates)
        {
            return new Inhabitant(Name,Life) {Place = coordinates};
        }
    }
}
