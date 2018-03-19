using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Messages;

namespace GameArea.Serialization
{
    /// <summary>
    /// Generic class which serializes/deserializes the message
    /// </summary>
    /// <typeparam name="T">Type of the input message</typeparam>
    public class MessageSerializator<T>
        where T: class
    {
        public Type MessageType { get; }

        public MessageSerializator()
        {
            MessageType = typeof(T);
        }

        public string Serialize(T messageObject)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (StringWriter textWriter = new StringWriter())
            {
                try
                {
                    serializer.Serialize(textWriter, messageObject);
                    return textWriter.ToString();
                }
                catch(Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Cannot serialize object");
                    return "";
                }
            }

            //FileStream fs = new FileStream("tempMessageFile.xml", FileMode.Create);
            //TextWriter writer = new StreamWriter(fs, new UTF8Encoding());

            //serializer.Serialize(writer, messageObject);
            //writer.Close();
            //fs.Close();

            //fs = new FileStream("tempMessageFile.xml", FileMode.Open);
            //TextReader reader = new StreamReader(fs, new UTF8Encoding());
            //var xmlString = reader.ReadToEnd();
            //reader.Close();
            //fs.Close();

            //File.Delete("tempMessageFile.xml");

            //return xmlString;
        }

        public T Deserialize(string message)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            T deserializedResult = null;
            try
            {
                deserializedResult = xs.Deserialize(new StringReader(message)) as T;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Cannot deserialize string");
                return null;
            }

            return deserializedResult;

            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.LoadXml(message);

            //var rootNode = xmlDoc.SelectSingleNode("/");
            //var rootNodeName = rootNode.Name;

            //Type messageType = null;

            //try
            //{
            //    messageType = Type.GetType(rootNodeName);
            //}
            //catch(Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine("Cannot resolve the message type");
            //    return null;
            //}

            //if (MessageType == messageType)
            //{
            //    var messageObject = Activator.CreateInstance(typeof(T));

            //    XmlSerializer xs = new XmlSerializer(typeof(T));
            //    T deserializedResult = null;
            //    try
            //    {
            //        deserializedResult = xs.Deserialize(new StringReader(message)) as T;
            //    }
            //    catch(Exception e)
            //    {
            //        System.Diagnostics.Debug.WriteLine("Cannot deserialize string");
            //        return null;
            //    }

            //    return deserializedResult;
            //}


            //return null;
        }

        //public bool CanDeserialize(string message)
        //{
        //    return null;
        //}

    }
}
