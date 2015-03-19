using System;
using System.Collections.Generic;
using System.Linq;
using ForestInhabitants.ForestObjects;

namespace ForestInhabitants
{  

    public class Forest
    {
        public List<List<ForestObject>> Map = new List<List<ForestObject>>();
        public HashSet<Inhabitant> Inhabitants = new HashSet<Inhabitant>();
        public HashSet<Inhabitant> DestroyedInhabitants = new HashSet<Inhabitant>();
        public event Action<Forest> ForestChange;                  //   Событие изменения леса
        public event Func<Inhabitant,bool> InhabitantCreated;      //        События
        public event Func<Inhabitant, bool> InhabitantDestroyed;   //         лесных
        public event Action InhabitantMove;        //        жителей

        public bool CreateInhabitant(ref Inhabitant inhabitant,Coordinates place,Coordinates purpose)
        {
            if (OutOfBorders(place) || OutOfBorders(purpose) || purpose.Equals(place))
                return false;
            if (inhabitant.EnterIntoLife)
                inhabitant.EnterIntoLife = false;
            var successChange = Map[place.Y][place.X].CanEnter(ref inhabitant, ref Map, place);           
            if (successChange && Map[purpose.Y][purpose.X].CanMove)
                 inhabitant.Purpose = purpose;
            return successChange && OnInhabitantCreated(inhabitant) && Inhabitants.Add(inhabitant) && DestroyInhabitantsWithZeroLife();
        }
         
        public bool Move(ref Inhabitant inhabitant, Coordinates command)
        {            
            var newPosition = command.Add(inhabitant.Place);
            if (OutOfBorders(newPosition)) 
                return false;
            var prevObject = inhabitant.PrevObject;
            if (inhabitant.EnterIntoLife)
                inhabitant.EnterIntoLife = false;
            var successChange = Map[newPosition.Y][newPosition.X].CanEnter(ref inhabitant, ref Map, newPosition) && inhabitant.Leave(ref Map, prevObject);
            if (successChange && inhabitant.Life > 0)
                OnInhabitantMove();
            return successChange && DestroyInhabitantsWithZeroLife();
        }

        public bool DestroyInhabitant(ref Inhabitant inhabitant)
        {
            Inhabitants.Remove(inhabitant);
            DestroyedInhabitants.Add(inhabitant);
            Map[inhabitant.Place.Y][inhabitant.Place.X] = inhabitant.PrevObject;
            OnInhabitantDestroyed(inhabitant);
            return true;
        }  

        private bool OutOfBorders(Coordinates position)
        {
            if (position == null)
                return true;
            return position.X < 0 || position.Y >= Map.Count || position.Y < 0 || position.X >= Map[position.Y].Count;
        }

        private void OnForestChange()
        {
            if (ForestChange != null)
                ForestChange(this);
        }
        private bool OnInhabitantCreated(Inhabitant inhabitant)
        {
            if (InhabitantCreated != null) 
                InhabitantCreated(inhabitant);
            OnForestChange();
            return true;
        }
        private void OnInhabitantDestroyed(Inhabitant inhabitant)
        {
            if (InhabitantDestroyed != null)
                InhabitantDestroyed(inhabitant);
        }

        private void OnInhabitantMove()
        {
            if (InhabitantMove != null)
                InhabitantMove();
            OnForestChange();
        }

        private bool DestroyInhabitantsWithZeroLife()
        {           
            while (Inhabitants.Any(inhabitant => inhabitant.Life <= 0))
            {
                var destroyedInhabitant = Inhabitants.First(inhabitant => inhabitant.Life <= 0);                
                DestroyInhabitant(ref destroyedInhabitant);
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
