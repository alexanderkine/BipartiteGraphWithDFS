using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestInhabitants.ForestObjects
{
    public class Bush : ForestObject
    {
        public Bush()
        { }
        public Bush(Coordinates place) : base(place) { }

        public override bool CanMove { get { return false; } }

        public override bool CanEnter(ref Inhabitant inhabitant, ref List<List<ForestObject>> map, Coordinates place)
        {
            return false;
        }

        public override ForestObject CoordinateObject(Coordinates coordinates)
        {
            return new Bush(coordinates);
        }
    }   
}
