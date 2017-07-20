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
using System.Runtime.Serialization;
using System.Collections;
using System.Data;

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

        public static XElement DataContractToXml(object data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            XDocument doc = new XDocument();
            using (XmlWriter writer = doc.CreateWriter())
            {
                DataContractSerializer ser = new DataContractSerializer(data.GetType());
                ser.WriteObject(writer, data);
                writer.Flush();
            }
            return doc.Root;

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

        #region 对象转换
        /// <summary>
        /// Hashtable
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        public static Hashtable XMLToHashtable(string xmlData)
        {
            DataTable dt = XMLToDataTable(xmlData);
            return DataTableHelper.DataTableToHashtable(dt);
        }
        /// <summary>
        /// DataTable
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        public static DataTable XMLToDataTable(string xmlData)
        {
            if (!String.IsNullOrEmpty(xmlData))
            {
                DataSet ds = new DataSet();
                ds.ReadXml(new System.IO.StringReader(xmlData));
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];
            }
            return null;
        }
        /// <summary>
        /// DataSet
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        public static DataSet XMLToDataSet(string xmlData)
        {
            if (!String.IsNullOrEmpty(xmlData))
            {
                DataSet ds = new DataSet();
                ds.ReadXml(new System.IO.StringReader(xmlData));
                return ds;
            }
            return null;
        }
        #endregion

        #region 增、删、改操作
        /// <summary>
        /// 追加节点
        /// </summary>
        /// <param name="filePath">XML文档绝对路径</param>
        /// <param name="xPath">范例: @"Skill/First/SkillItem"</param>
        /// <param name="xmlNode">XmlNode节点</param>
        /// <returns></returns>
        public static bool AppendChild(string filePath, string xPath, XmlNode xmlNode)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                XmlNode xn = doc.SelectSingleNode(xPath);
                XmlNode n = doc.ImportNode(xmlNode, true);
                xn.AppendChild(n);
                doc.Save(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 从XML文档中读取节点追加到另一个XML文档中
        /// </summary>
        /// <param name="filePath">需要读取的XML文档绝对路径</param>
        /// <param name="xPath">范例: @"Skill/First/SkillItem"</param>
        /// <param name="toFilePath">被追加节点的XML文档绝对路径</param>
        /// <param name="toXPath">范例: @"Skill/First/SkillItem"</param>
        /// <returns></returns>
        public static bool AppendChild(string filePath, string xPath, string toFilePath, string toXPath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(toFilePath);
                XmlNode xn = doc.SelectSingleNode(toXPath);

                XmlNodeList xnList = ReadNodes(filePath, xPath);
                if (xnList != null)
                {
                    foreach (XmlElement xe in xnList)
                    {
                        XmlNode n = doc.ImportNode(xe, true);
                        xn.AppendChild(n);
                    }
                    doc.Save(toFilePath);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 修改节点的InnerText的值
        /// </summary>
        /// <param name="filePath">XML文件绝对路径</param>
        /// <param name="xPath">范例: @"Skill/First/SkillItem"</param>
        /// <param name="value">节点的值</param>
        /// <returns></returns>
        public static bool UpdateNodeInnerText(string filePath, string xPath, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                XmlNode xn = doc.SelectSingleNode(xPath);
                XmlElement xe = (XmlElement)xn;
                xe.InnerText = value;
                doc.Save(filePath);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 读取XML文档
        /// </summary>
        /// <param name="filePath">XML文件绝对路径</param>
        /// <returns></returns>
        public static XmlDocument LoadXmlDoc(string filePath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                return doc;
            }
            catch
            {
                return null;
            }
        }
        #endregion 增、删、改操作

        #region 扩展方法
        /// <summary>
        /// 读取XML的所有子节点
        /// </summary>
        /// <param name="filePath">XML文件绝对路径</param>
        /// <param name="xPath">范例: @"Skill/First/SkillItem"</param>
        /// <returns></returns>
        public static XmlNodeList ReadNodes(string filePath, string xPath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                XmlNode xn = doc.SelectSingleNode(xPath);
                XmlNodeList xnList = xn.ChildNodes;  //得到该节点的子节点
                return xnList;
            }
            catch
            {
                return null;
            }
        }

        #endregion 扩展方法
    }
}
