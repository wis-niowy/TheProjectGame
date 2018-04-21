using Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GameArea.Parsers
{
    public class MessageParser : IParser
    {
        public MessageParser()
        { }

        public string SerializeObjectToXml<T>(T msg)
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

        public T DeserializeXmlToObject<T>(string xml)
            where T: class
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(xml))
            {
                return (T)ser.Deserialize(sr);
            }
        }
    }
}
