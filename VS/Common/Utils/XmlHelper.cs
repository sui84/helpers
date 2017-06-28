using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Text;

namespace Common.Utils{

    public class XmlHelper
    {
        public static string ObjToXmlStr(Object obj)
        {
            string xmlStr = string.Empty;
            //XmlSerializer xmlser = new XmlSerializer(obj.GetType());
            //using (StringWriter sw = new StringWriter())
            //{
            //    xmlser.Serialize(sw, obj);
            //    xmlStr = sw.ToString();
            //};
              
           using(MemoryStream ms = new MemoryStream()){
               StreamWriter sw = new StreamWriter(ms);
               XmlWriterSettings setting = new XmlWriterSettings();
               setting.Encoding = Encoding.UTF8 ;
               setting.Indent = true ;
             //  XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
              // ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
               using (XmlWriter writer = XmlWriter.Create(sw, setting))
               {
                   XmlSerializer xmlser = new XmlSerializer(obj.GetType());
                   xmlser.Serialize(writer, obj );//,ns);
                   writer.Flush();
                   writer.Close();
               }
               using (StreamReader sr = new StreamReader(ms))
               {
                   ms.Position = 0;
                   xmlStr = sr.ReadToEnd();
                   sr.Close();
               }
           }
           return xmlStr; 
                
        }


        public static void ObjToXmlFile(Object obj , string filePath)
        {
            XmlSerializer xmlser = new XmlSerializer(obj.GetType());
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            xmlser.Serialize(fs, obj);
            fs.Close();
        }

        public static Object XmlFileToObj( string filePath)
        {
            Object obj = null;
            XmlDocument xmlDoc2 = new XmlDocument();
            xmlDoc2.Load(filePath);
            using (StringReader sr = new StringReader(xmlDoc2.InnerXml))
            {
                XmlSerializer xmldes = new XmlSerializer(typeof(Object));
                obj = (Object)xmldes.Deserialize(sr);
            }
            return obj;
        }

        public static Object XmlStrToObj(string xmlStr, Type type)
        {
            object obj = new object();
            using (StringReader sr = new StringReader(xmlStr))
            {
                XmlSerializer xmldes = new XmlSerializer(type);
                obj = xmldes.Deserialize(sr);
            }
            return obj;
        }

        public static XElement GetXEleByName(string xmlStr, string eleName)
        {
            XDocument xDoc = XDocument.Parse(xmlStr);
            IEnumerable<XElement> xEles = xDoc.Root.Elements();
            XElement xele = GetXEleByName(xEles, eleName);
            return xele;
        }

        public static XElement GetXEleByName(IEnumerable<XElement> xEles , string eleName)
        {
            var q = from item in xEles
                    where item.Name.LocalName == eleName
                    select item;
            return q.FirstOrDefault();
        }

        public static IEnumerable<XElement> GetXEleListByName(IEnumerable<XElement> xEles, string eleName)
        {
            var q = from item in xEles
                    where item.Name.LocalName == eleName
                    select item;
            return q.ToList();
        }

        public static void SetXEleValueByName(IEnumerable<XElement> xEles , string eleName , string eleValue)
        {
            XElement xele = GetXEleByName(xEles, eleName);
            SetXEleValue(xele, eleValue);
        }

        public static string GetXEleValueByName(IEnumerable<XElement> xEles, string eleName)
        {
            XElement xele = GetXEleByName(xEles, eleName);
            if (xele != null) return xele.Value ;
            return string.Empty;
        }



        public static void SetXEleValue(XElement xele, string eleValue)
        {
            if (xele != null) xele.Value = eleValue;
        }

        public static string ReplaceElement(string xmlStr, string eleName)
        {
            string eleValue = "**********";
            return ReplaceElement(xmlStr, eleName, eleValue);
        }

        public static string ReplaceElement(string xmlStr, string eleName, string eleValue)
        {
            XDocument xDoc = XDocument.Parse(xmlStr);
            IEnumerable<XElement> xEles = xDoc.Root.Elements();
            XmlHelper.SetXEleValueByName(xEles, eleName, eleValue);
            return xDoc.ToString();
        }
   
    }
}
