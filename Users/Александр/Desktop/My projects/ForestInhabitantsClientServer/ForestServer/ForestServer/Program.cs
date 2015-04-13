using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Security.Cryptography;
using System.Speech.Recognition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ForestServer.ForestObjects;
using Newtonsoft.Json;
namespace ForestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new ForestServer("forest.xml");
            server.Start();
        }
    }
}
