using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using ForestInhabitants.ForestObjects;

namespace ForestInhabitants.Generators
{
    class UserCommandGenerator : IForestCommandGenerator
    {
        public Coordinates GenerateCommand()
        {
            var key = Console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    return MoveCommand.Left;
                case ConsoleKey.RightArrow:
                    return MoveCommand.Right;
                case ConsoleKey.DownArrow:
                    return MoveCommand.Down;
                case ConsoleKey.UpArrow:
                    return MoveCommand.Up;
            }
            return null;
        }

        public void GenerateCommands(Forest forest)
        {
            var rnd = new Random();
            var canEnterObjects = new List<ForestObject>();
            foreach (var e in forest.Map)
                canEnterObjects.AddRange(e.FindAll(x => x.CanMove));
            while (true)
            {
                var randomObject = canEnterObjects[rnd.Next(canEnterObjects.Count)];
                var newInhabitant = new Inhabitant(RandomStringGenerator(rnd.Next(1,20)), rnd.Next(1, 20));
                forest.CreateInhabitant(ref newInhabitant, randomObject.Place, new Coordinates(0, 1));
                canEnterObjects.Remove(randomObject);
                while (!Console.NumberLock)
                {
                    while (forest.Inhabitants.Count != 0)
                    {
                        var randomInhabitant = forest.Inhabitants.ElementAt(rnd.Next(forest.Inhabitants.Count));
                        forest.Move(ref randomInhabitant, GenerateCommand());
                        canEnterObjects.Remove(randomInhabitant.PrevObject);
                        break;
                    }
                    Thread.Sleep(200);
                }
                Thread.Sleep(100);
            }
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
            //if (chars.Length == 1)
            //    return RandomStringGenerator(length);
            return (new string(chars));
        } 
    }
}
