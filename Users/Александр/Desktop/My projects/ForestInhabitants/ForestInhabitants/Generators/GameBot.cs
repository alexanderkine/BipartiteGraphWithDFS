using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using ForestInhabitants.ForestObjects;
using NUnit.Framework.Constraints;

namespace ForestInhabitants.Generators
{
    public class GameBot : IForestCommandGenerator
    {
        private Inhabitant realInhabitant;
        private Forest forest;
        private List<List<int>> mapWithWarFog;
        private bool wayFound;
        private readonly List<Coordinates> commands = new List<Coordinates> { MoveCommand.Right, MoveCommand.Up,  MoveCommand.Left , MoveCommand.Down};
        public Coordinates GenerateCommand()
        {
            var rnd = new Random();
            return commands[rnd.Next(0, 3)];
        }

        private Coordinates GenerateReverseCommand(Coordinates command)
        {
            if (command.Equals(MoveCommand.Right))
                return MoveCommand.Left;
            if (command.Equals(MoveCommand.Up))
                return MoveCommand.Down;
            if (command.Equals(MoveCommand.Left))
                return MoveCommand.Right;
            if (command.Equals(MoveCommand.Down))
                return MoveCommand.Up;
            return null;
        }
        public void GenerateCommands(Forest forest)
        {            
            var rnd = new Random();
            this.forest = forest;
            CreateWarFog();
            forest.InhabitantMove += () => Thread.Sleep(200);
            while (true)
            {
                CreateWarFog();
                CreateInhabitantOnTheRandomPlace(rnd);
                TryReachThePurpose();
            }
        }

        private void CreateWarFog()
        {
            mapWithWarFog =
                Enumerable.Range(0, forest.Map.Count)
                    .Select(x => Enumerable.Range(0, forest.Map[0].Count).Select(y => 0).ToList()).ToList();
        }

        private void CreateInhabitantOnTheRandomPlace(Random rnd)
        {
            var canEnterObjects = new List<ForestObject>();
            foreach (var rowForestObjects in forest.Map)
                canEnterObjects.AddRange(rowForestObjects.FindAll(x => x.CanMove));
            var randomObject = canEnterObjects[rnd.Next(canEnterObjects.Count)];
            var purpose = canEnterObjects[rnd.Next(canEnterObjects.Count)];
            realInhabitant = new Inhabitant(RandomStringGenerator(rnd.Next(4, 8)), rnd.Next(1, 7));
            forest.CreateInhabitant(ref realInhabitant, randomObject.Place, purpose.Place); ;
        }

        private void TryReachThePurpose()
        {
            TryMove(realInhabitant.Place);
            commandsStack.Clear();
            wayFound = false;
            forest.DestroyInhabitant(ref realInhabitant);
        }

        private Stack<Coordinates> commandsStack = new Stack<Coordinates>();

        private void TryMove(Coordinates currentPlace)
        {
            mapWithWarFog[currentPlace.Y][currentPlace.X] = 1;
            foreach (var command in commands)
            {
                var nextPlace = realInhabitant.Place.Add(command);
                if (OutOfBorders(nextPlace)) continue;
                if (mapWithWarFog[nextPlace.Y][nextPlace.X] == 0 && !wayFound)
                {
                    if (!forest.Move(ref realInhabitant, command))
                    {
                        mapWithWarFog[nextPlace.Y][nextPlace.X] = 1;
                        continue;
                    }
                    commandsStack.Push(command);
                    if (nextPlace.Equals(realInhabitant.Purpose))
                    {
                        wayFound = true;
                        return;
                    }
                    TryMove(nextPlace);
                    if (wayFound)
                        return;
                    var currentCommand = commandsStack.Pop();
                    forest.Move(ref realInhabitant, GenerateReverseCommand(currentCommand));
                }
            }
        }

        private bool OutOfBorders(Coordinates position)
        {
            if (position == null)
                return true;
            return position.X < 0 || position.Y >= forest.Map.Count || position.Y < 0 || position.X >= forest.Map[position.Y].Count;
        }

        private string RandomStringGenerator(int length)
        {
            var rng = RandomNumberGenerator.Create();
            var chars = new char[length];
            var validChars = "abcdefghijklmnopqrstuvwxyzABCEDFGHIJKLMNOPQRSTUVWXYZ1234567890";
            for (var i = 0; i < length; i++)
            {
                var bytes = new byte[1];
                rng.GetBytes(bytes);
                var rnd = new Random(bytes[0]);
                chars[i] = validChars[rnd.Next(0, 61)];
            }
            return (new string(chars));
        }        
    }
}
