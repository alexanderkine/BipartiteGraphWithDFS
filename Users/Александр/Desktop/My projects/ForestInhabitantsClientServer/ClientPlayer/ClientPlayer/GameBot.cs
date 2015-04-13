﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using ClientPlayer.ForestObjects;
using Newtonsoft.Json;

namespace ClientPlayer
{
    public class GameBot
    {
        private Inhabitant inhabitant;
        private readonly Socket socket;
        private readonly int mapHeight;
        private readonly int mapWidth;
        private int[][] mapWithWarFog;
        private bool wayFound;

        private readonly List<Coordinates> commands = new List<Coordinates>
        {
            MoveCommand.Right,
            MoveCommand.Up,
            MoveCommand.Left,
            MoveCommand.Down
        };

        public GameBot(Inhabitant inhabitant,Socket socket)
        {
            this.inhabitant = inhabitant;
            this.socket = socket;
            var buffer = new byte[512];
            socket.Receive(buffer);
            var size = Encoding.UTF8.GetString(buffer).Replace("\0", "").Split(' ');
            mapHeight = int.Parse(size[0]);
            mapWidth = int.Parse(size[1]);
            CreateWarFog();
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

        private void CreateWarFog()
        {
            mapWithWarFog =
                Enumerable.Range(0, mapHeight)
                    .Select(x => Enumerable.Range(0, mapWidth).Select(y => 0).ToArray()).ToArray();
        }

        public void TryReachThePurpose()
        {
            TryMove(inhabitant.Place);
            commandsStack.Clear();
            wayFound = false;
        }

        private readonly Stack<Coordinates> commandsStack = new Stack<Coordinates>();

        private void TryMove(Coordinates currentPlace)
        {
            mapWithWarFog[currentPlace.Y][currentPlace.X] = 1;
            foreach (var command in commands)
            {
                var nextPlace = inhabitant.Place.Add(command);
                if (OutOfBorders(nextPlace)) continue;
                if (mapWithWarFog[nextPlace.Y][nextPlace.X] == 0 && !wayFound)
                {
                    if (!Move(command))
                    {
                        mapWithWarFog[nextPlace.Y][nextPlace.X] = 1;
                        continue;
                    }
                    commandsStack.Push(command);
                    if (nextPlace.Equals(inhabitant.Purpose))
                    {
                        wayFound = true;
                        return;
                    }
                    TryMove(nextPlace);
                    if (wayFound)
                        return;
                    var currentCommand = commandsStack.Pop();
                    Move(GenerateReverseCommand(currentCommand));
                }
            }
        }

        private bool Move(Coordinates command)
        {
            var buffer = new byte[256];
            var ser = JsonConvert.SerializeObject(command,Formatting.Indented);
            socket.Send(Encoding.UTF8.GetBytes(ser));
            socket.Receive(buffer);
            var serInhabitant = Encoding.UTF8.GetString(buffer).Replace("\0", "");
            var oldCoordinates = inhabitant.Place;
            inhabitant = JsonConvert.DeserializeObject<Inhabitant>(serInhabitant, new ForestObjectConverter());
            return !oldCoordinates.Equals(inhabitant.Place);
        }

        private bool OutOfBorders(Coordinates position)
        {
            if (position == null)
                return true;
            return position.X < 0 || position.Y >= mapHeight || position.Y < 0 || position.X >= mapWidth;
        }
    }
}
