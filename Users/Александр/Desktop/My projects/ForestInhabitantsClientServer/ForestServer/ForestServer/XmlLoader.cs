using System.IO;
using System.Xml.Serialization;
using ForestServer.ForestObjects;

namespace ForestServer
{
    public static class XmlLoader
    {
        private static readonly XmlSerializer forestFormatter = new XmlSerializer(typeof(Forest), new[] { typeof(Bush), typeof(Trap), typeof(Inhabitant), typeof(Life), typeof(Footpath) });
        public static void SaveData(string path, object obj)
        {
            if (obj is Forest)
            {
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    forestFormatter.Serialize(fs, (obj as Forest));
                }
            }
        }
        public static  Forest DeserializeForest(string path)
        {
            Forest forest;
            using (var fs = new FileStream(path, FileMode.Open))
                  forest = (Forest)forestFormatter.Deserialize(fs);
            return forest;
        }
    }
}
