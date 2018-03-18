using System;
using System.IO;
using System.Text;
using System.Xml;

public class Logger
{
    private string path; // csv path

    public Logger(string path)
    {
        this.path = path;
    }

    public void Log(XmlDocument doc)
    {
        using (StreamWriter sw = File.AppendText(path))
        {
            StringBuilder sb = new StringBuilder();
            
            foreach (XmlNode node in doc.ChildNodes)
            {
                if (node.Attributes == null) continue;
                sb.Append(node.Name + '\t');

                DateTime dt = DateTime.Now;
                string str = String.Format("{0:yyyy-mm-ddTHH:mm:ss.fff}", dt);
                sb.Append(str + '\t');

                var nameAttr = node.Attributes["gameId"];
                if (nameAttr != null)
                    sb.Append(nameAttr.InnerText + '\t');

                nameAttr = node.Attributes["playerGuid"];
                if (nameAttr != null)
                    sb.Append(nameAttr.InnerText + '\n');
                
                // TODO
                // add Player ID, Colour, Role
                // more information about GM is required
            }

            //Console.WriteLine(sb);
            sw.Write(sb.ToString());
        }
    }

}

