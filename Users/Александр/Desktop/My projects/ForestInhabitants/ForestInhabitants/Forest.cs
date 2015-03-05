using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ForestInhabitants.ForestObjects;

namespace ForestInhabitants
{  

    public class Forest
    {
        public List<List<ForestObject>> Map = new List<List<ForestObject>>();
        public HashSet<Inhabitant> Inhabitants = new HashSet<Inhabitant>();
        public event Action<Forest> ForestChange;
        public bool CreateInhabitant(Inhabitant inhabitant,Coordinates place)
        {
            if (OutOfBorders(place))
                return false;
            var successChange = Map[place.Y][place.X].CanEnter(ref inhabitant, ref Map, place);
            OnForestChange();
            return successChange && Inhabitants.Add(inhabitant) && DestroyInhabitant();
        }        
          
        public bool Move(Inhabitant inhabitant, Coordinates command)
        {            
            var newPosition = command.Add(inhabitant.Place);
            if (OutOfBorders(newPosition)) 
                return false;
            var prevObject = inhabitant.PrevObject;
            var successChange = Map[newPosition.Y][newPosition.X].CanEnter(ref inhabitant, ref Map, newPosition) && inhabitant.Leave(ref Map, prevObject);
            OnForestChange();
            return successChange && DestroyInhabitant();
        }

        private bool OutOfBorders(Coordinates position)
        {
            return position.X < 0 || position.Y >= Map.Count || position.Y < 0 || position.X >= Map[position.Y].Count;
        }

        private void OnForestChange()
        {
            if (ForestChange != null)
                ForestChange(this);
        }
        private bool DestroyInhabitant()
        {
            while (Inhabitants.Any(inhabitant => inhabitant.Life <= 0))
            {
                var destroyedInhabitant = Inhabitants.First(inhabitant => inhabitant.Life <= 0);
                Inhabitants.Remove(destroyedInhabitant);
                Map[destroyedInhabitant.Place.Y][destroyedInhabitant.Place.X] = destroyedInhabitant.PrevObject;
                OnForestChange();
            }
            return true;
        }
    }

    public class MoveCommand
    {
        public static Coordinates Up = new Coordinates(0,-1);
        public static Coordinates Down = new Coordinates(0, 1);
        public static Coordinates Left = new Coordinates(-1, 0); 
        public static Coordinates Right = new Coordinates(1, 0);
    }
}
