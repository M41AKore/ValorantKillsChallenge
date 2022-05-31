using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValorantKillsChallenge
{
    public static class XmlSerializer
    {
        public static T deserializeXml<T>(string path)
        {
            try
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(T));
                System.IO.StreamReader file = new System.IO.StreamReader(path);
                var obj = (T)reader.Deserialize(file);
                file.Close();
                return obj;
            }
            catch (Exception ex)
            {
                Console.WriteLine("XmlSerializer Exception: " + Environment.NewLine + ex.Message);
            }

            return default(T);
        }

        public static void serializeToXml<T>(T obj, string path)
        {
            var writer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            var wfile = new System.IO.StreamWriter(path);
            writer.Serialize(wfile, obj);
            wfile.Close();
        }
    }
}
