using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestInhabitants.ForestObjects
{
    public class Life : ForestObject
    {
        public Life()
        { }
        public Life(Coordinates place) : base(place) { }

        public override bool CanMove { get { return true; } }

        public override bool CanEnter(ref Inhabitant inhabitant, ref List<List<ForestObject>> map, Coordinates place)
        {
            inhabitant.PrevObject = new Footpath(place);
            map[place.Y][place.X] = inhabitant;
            inhabitant.Place = place;
            ++inhabitant.Life;
            return true; 
        }

        public override ForestObject CoordinateObject(Coordinates coordinates)
        {
            return new Life(coordinates);
        }

        public override char ToChar()
        {
            return '\u2665';
        }
    }
}
