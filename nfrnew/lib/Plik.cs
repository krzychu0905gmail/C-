using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using System.Xml.Serialization;
//using System.Xml;
using System.Runtime.Serialization;

namespace nfrnew.lib
{
    public class Plik
    {
        public AfaControlConf AfaContrConf;
        string filePath = string.Empty;

        public Plik(string _filePath)
        {
            AfaContrConf = new AfaControlConf();

            filePath = _filePath;
        }

        public enum User
        {
            USER = 0,
            ADMIN = 1
        };


        public class Contact
        {
            public string Name = "Krzysztof Kieronski", Number = "+48535159626";

            public Contact()
            {

            }

            public Contact(string Name, string Num)
            {
                this.Name = Name;
                this.Number = Num;
            }

            public void Change(string Name, string Num)
            {
                this.Name = Name;
                this.Number = Num;
            }
        }


        [DataContract]
        public class AfaControlConf
        {
            [DataMember]
            public User AppUser;
            [DataMember]
            public int lastDevAddress;
            [DataMember]
            public bool consoleEnable;
            [DataMember]
            public string LastComName = "COM1";
            [DataMember]
            public string LastContactKey = string.Empty;
            [DataMember]
            public Dictionary<string, Contact> Phonebook = new Dictionary<string, Contact>();

        }

        public string Open()
        {
            string all = "ERROR READ FILE " + filePath;

            if (File.Exists(filePath))
            {
                try
                {
                    string outs = File.ReadAllText(filePath, Encoding.UTF8);
                    AfaContrConf = (AfaControlConf)Deserialize(outs, AfaContrConf.GetType());
                    //Console.WriteLine(outs);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                Console.WriteLine("Deserialized: " + filePath);
            }
            else 
            {

                string outs = Serialize(AfaContrConf);
                if (!File.Exists(filePath))
                {
                    //File.WriteAllText(filePath, )
                    StreamWriter sw = new StreamWriter(filePath, true, Encoding.UTF8);
                    sw.WriteLine(outs);
                    sw.Close();

                    Console.WriteLine("Create new xml file");
                }
            }

            return all;
        }


        public void SaveParam()
        {
            string outs = Serialize(AfaContrConf);

            StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8);
            sw.WriteLine(outs);
            sw.Close();
            Console.WriteLine("Save param to xml file");
            Console.WriteLine("");
        }


        public static string Serialize(object obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(memoryStream, obj);
                memoryStream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        public static object Deserialize(string xml, Type toType)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(toType);
                return deserializer.ReadObject(stream);
            }
        }

    }
}
