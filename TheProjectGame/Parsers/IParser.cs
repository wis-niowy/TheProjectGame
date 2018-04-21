using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.Parsers
{
    public interface IParser
    {
        T DeserializeXmlToObject<T>(string xml) where T: class;
        string SerializeObjectToXml<T>(T msg) where T: class;
    }
}
