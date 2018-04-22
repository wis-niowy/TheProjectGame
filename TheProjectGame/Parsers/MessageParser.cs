using Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GameArea.Parsers
{
    public class MessageParser
    {
        public MessageParser()
        { }

        public static object DeserializeXML(string xml)
        {
            //napisac tu analogicznie aprsowanie każdego typu wiadomości na podstawie MessageReader z communication server

            throw new NotImplementedException("deserializeXML - na object analogicznie do communication server");
            return null;
        }

        public static string SerializeObjectToXml<T>(T msg)
            where T: class
        {
            if (msg == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, msg);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        public static T DeserializeXmlToObject<T>(string xml)
            where T: class
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(xml))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        public static string[] SerializeObjectsToXml<T>(IEnumerable<T> msgs)
            where T:class
        {
            List<string> xmls = new List<string>();
            foreach(var msg in msgs)
            {
                xmls.Add(SerializeObjectToXml(msg));
            }
            return xmls.ToArray();
        }
    }
}
