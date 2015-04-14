using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using ForestServer.ForestObjects;
using Newtonsoft.Json;

namespace ForestServer
{
    public class ForestServer
    {
        private Forest forest;
        private readonly int numberOfPlayers;
        private readonly List<Socket> players = new List<Socket>();
        private readonly List<Socket> visualisators = new List<Socket>();
        private readonly List<Inhabitant> reservedInhabitants = new List<Inhabitant>(); 
        private readonly List<Thread> bots = new List<Thread>();

        public ForestServer(string forestPath,string numberOfPlayersPath)
        {
            var buffer = new byte[64];
            using (var fs = File.OpenRead(numberOfPlayersPath))
            {
                fs.Read(buffer, 0, buffer.Length);
            }
            numberOfPlayers = int.Parse(Encoding.UTF8.GetString(buffer));
            //XmlLoader.SaveData(forestPath, Load(@"Maps\Maze3.txt"));
            forest = XmlLoader.DeserializeForest(forestPath);
        }

        public void Start(string ipAddr,int port)
        {
            var endPoint = new IPEndPoint(IPAddress.Parse(ipAddr), port);
            var listener = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(endPoint);
            listener.Listen(10);
            Console.WriteLine("Прослушиваю порт {0}:",port);
            WaitConnections(listener);
            SendForestToVisualisators();
            foreach (var player in players)
            {
                reservedInhabitants.Add(BeforeStart(player));
                bots.Add(new Thread(Worker));
            }
            for (var i = 0; i < reservedInhabitants.Count; i++)
                bots[i].Start(i);              
            while (bots.Any(worker => worker.IsAlive)) { }
            Thread.Sleep(1000);
            foreach (var vis in visualisators)
            {
                vis.Shutdown(SocketShutdown.Both);
                vis.Close();
            }
        }

        private void Worker(object i)
        {
            var iLocal = (int) i;
            lock (reservedInhabitants)
            {
                AfterStart(reservedInhabitants[iLocal], players[iLocal]);
            }
        }

        private void WaitConnections(Socket listener)
        {
            while (visualisators.Count == 0 || players.Count != numberOfPlayers)
            {
                var buffer = new byte[128];
                var socket = listener.Accept();
                if (socket.Connected)
                {
                    socket.Receive(buffer);
                    if (Encoding.UTF8.GetString(buffer).Replace("\0", "").Equals("I am player"))
                    {
                        Console.WriteLine("Игрок подключился");
                        players.Add(socket);
                    }
                    else if (Encoding.UTF8.GetString(buffer).Replace("\0", "").Equals("I am visualisator"))
                    {
                        Console.WriteLine("Визуализатор подключился");
                        visualisators.Add(socket);
                    }
                }
            }
        }
        private void SendForestToVisualisators()
        {
            byte[] buffer;
            XmlLoader.SaveData("forestNow.xml", forest);
            using (var fs = new StreamReader("forestNow.xml"))
            {
                buffer = Encoding.UTF8.GetBytes(fs.ReadToEnd());
            }
            foreach (var vis in visualisators)
                vis.Send(buffer, buffer.Length, SocketFlags.None);
        }

        private Inhabitant BeforeStart(Socket player)
        {
            var buffer = new byte[1024];
            player.Send(Encoding.UTF8.GetBytes(string.Join(" ", forest.Inhabitants
                .Where(y => !reservedInhabitants.Contains(y))
                .Select(x => x.Name)
                .ToArray())));
            player.Receive(buffer);
            var name = Encoding.UTF8.GetString(buffer).Replace("\0", "");
            var selectedInhabitant = forest.Inhabitants
                .First(x => x.Name.Equals(name));
            var serializedInhabitant = JsonConvert.SerializeObject(selectedInhabitant);
            player.Send(Encoding.UTF8.GetBytes(serializedInhabitant));
            Thread.Sleep(50);
            var size = string.Format("{0} {1}", forest.Map.Length, forest.Map[0].Length);
            player.Send(Encoding.UTF8.GetBytes(size));
            return selectedInhabitant;
        }

        private void AfterStart(Inhabitant inhabitant,Socket player)
        {
            string serializedInhabitant;
            while (!inhabitant.Place.Equals(inhabitant.Purpose))
            {
                var buffer = new byte[32];
                player.Receive(buffer);
                var data = Encoding.UTF8.GetString(buffer
                    .Where(y => y != 0)
                    .ToArray());
                var command = JsonConvert.DeserializeObject<Coordinates>(data);
                Thread.Sleep(200);
                if (forest.Move(ref inhabitant, command))
                {
                    Thread.Sleep(50);
                    if (inhabitant.Place.Equals(inhabitant.Purpose))
                        break;
                    if (inhabitant.Life <= 0)
                    {                      
                        EndConnectionWithPlayer(player);
                        return;
                    }
                    SendForestToVisualisators();
                    serializedInhabitant = JsonConvert.SerializeObject(inhabitant);
                    player.Send(Encoding.UTF8.GetBytes(serializedInhabitant));
                    if (bots.Count(worker => worker.IsAlive) > 1)
                    {
                        Monitor.Pulse(reservedInhabitants);
                        Monitor.Wait(reservedInhabitants);
                    }
                    continue;
                }
                serializedInhabitant = JsonConvert.SerializeObject(inhabitant);
                player.Send(Encoding.UTF8.GetBytes(serializedInhabitant));
            }
            forest.DestroyInhabitant(ref inhabitant);
            EndConnectionWithPlayer(player);
        }

        private void EndConnectionWithPlayer(Socket player)
        {
            SendForestToVisualisators();
            player.Shutdown(SocketShutdown.Both);
            player.Close();
            Monitor.Pulse(reservedInhabitants);
        }

        private void CreateInhabitantsOnTheRandomPlaces()
        {
            var rnd = new Random();
            for (var i = 0; i < numberOfPlayers; i++)
            {
                var canEnterObjects = new List<ForestObject>();
                foreach (var rowForestObjects in forest.Map)
                    canEnterObjects.AddRange(rowForestObjects.Where(forestObject => forestObject.CanMove));
                var randomObject = canEnterObjects[rnd.Next(canEnterObjects.Count)];
                canEnterObjects.Remove(randomObject);
                var purpose = canEnterObjects[rnd.Next(canEnterObjects.Count)];
                canEnterObjects.Add(randomObject);
                var inhabitant = new Inhabitant(RandomStringGenerator(rnd.Next(4, 8)), rnd.Next(2, 4));
                forest.CreateInhabitant(ref inhabitant, randomObject.Place, purpose.Place);
            }
        }


        private Forest Load(string path)
        {
            forest = new ForestLoader(new StreamReader(path)).Load();
            CreateInhabitantsOnTheRandomPlaces();
            return forest;
        }

        private static string RandomStringGenerator(int length)
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
