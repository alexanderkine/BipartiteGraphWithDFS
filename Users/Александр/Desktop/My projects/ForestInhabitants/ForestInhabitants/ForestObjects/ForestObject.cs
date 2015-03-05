using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestInhabitants.ForestObjects
{
    public abstract class ForestObject
    {
        public Coordinates Place;
        public abstract bool CanMove { get; }
        public ForestObject(Coordinates place)
        {
            Place = place;
        }

        protected ForestObject(){}

        public abstract bool CanEnter(ref Inhabitant inhabitant, ref List<List<ForestObject>> map, Coordinates place);

        public abstract ForestObject CoordinateObject(Coordinates coordinates);

        public abstract char ToChar();
    }
}
