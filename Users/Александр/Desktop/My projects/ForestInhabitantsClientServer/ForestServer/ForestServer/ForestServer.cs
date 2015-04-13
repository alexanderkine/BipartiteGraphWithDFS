using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ForestServer.ForestObjects;
using Newtonsoft.Json;

namespace ForestServer
{
    public class ForestServer
    {
        private Forest forest;
        private Socket player;
        private Socket visualisator;

        public ForestServer(string forestPath)
        {
            forest = XmlLoader.DeserializeForest(forestPath);          
        }

        public void Start()
        {
            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
            var listener = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(endPoint);
            listener.Listen(10);
            Console.WriteLine("Прослушиваю порт 11000:");
            player = listener.Accept();
            if (player.Connected)
            {
                Console.WriteLine("1 клиент соединился");
                visualisator = listener.Accept();
                if (visualisator.Connected)
                {
                    Console.WriteLine("2 клиент соединился");
                    SendForestToVisualisator();
                    var inhabitant = BeforeStart();
                    AfterStart(ref inhabitant);
                }
            }
        }
        private void SendForestToVisualisator()
        {
            byte[] buffer;
            XmlLoader.SaveData("forestNow.xml", forest);
            using (var fs = new StreamReader("forestNow.xml"))
            {
                buffer = Encoding.UTF8.GetBytes(fs.ReadToEnd());
            }
            visualisator.Send(buffer, buffer.Length, SocketFlags.None);
        }

        private Inhabitant BeforeStart()
        {
            var buffer = new byte[1024];
            player.Send(Encoding.UTF8.GetBytes(string.Join(" ", forest.Inhabitants
                .Where(y => y != null)
                .Select(x => x.Name)
                .ToArray())));
            player.Receive(buffer);
            var name = Encoding.UTF8.GetString(buffer).Replace("\0", "");
            var selectedInhabitant = forest.Inhabitants.Where(y => y != null).First(x => x.Name.Equals(name));
            var ser = JsonConvert.SerializeObject(selectedInhabitant);
            player.Send(Encoding.UTF8.GetBytes(ser));
            Thread.Sleep(50);
            var size = string.Format("{0} {1}", forest.Map.Length, forest.Map[0].Length);
            player.Send(Encoding.UTF8.GetBytes(size));
            return selectedInhabitant;
        }

        private void AfterStart(ref Inhabitant inhabitant)
        {
            while (!inhabitant.Place.Equals(inhabitant.Purpose))
            {
                var buffer = new byte[32];
                player.Receive(buffer);
                var data = Encoding.UTF8.GetString(buffer.Where(y => y != 0).ToArray());
                var command = JsonConvert.DeserializeObject<Coordinates>(data);
                Thread.Sleep(200);
                if (forest.Move(ref inhabitant, command))
                    SendForestToVisualisator();
                var ser = JsonConvert.SerializeObject(inhabitant);
                player.Send(Encoding.UTF8.GetBytes(ser));
            }
            forest.DestroyInhabitant(ref inhabitant);
            Thread.Sleep(200);
            SendForestToVisualisator();
            player.Shutdown(SocketShutdown.Both);
            player.Close();
        }

        private void CreateInhabitantsOnTheRandomPlaces(int count)
        {
            var rnd = new Random();
            for (var i = 0; i < count; i++)
            {
                var canEnterObjects = new List<ForestObject>();
                foreach (var rowForestObjects in forest.Map)
                    canEnterObjects.AddRange(rowForestObjects.Where(forestObject => forestObject.CanMove));
                var randomObject = canEnterObjects[rnd.Next(canEnterObjects.Count)];
                canEnterObjects.Remove(randomObject);
                var purpose = canEnterObjects[rnd.Next(canEnterObjects.Count)];
                canEnterObjects.Add(randomObject);
                var inhabitant = new Inhabitant(RandomStringGenerator(rnd.Next(4, 8)), rnd.Next(7, 8));
                forest.CreateInhabitant(ref inhabitant, randomObject.Place, purpose.Place);
            }
        }


        private Forest Load(string path)
        {
            forest = new ForestLoader(new StreamReader(path)).Load();
            CreateInhabitantsOnTheRandomPlaces(2);
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
