using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using Common.Utils;
using Common.Utils.Tibco;
using Common.Utils.Files;

namespace Common.Tests
{
    //1. Generate class to xml document
    //C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC>csc "C:\Program Files (x86)\Microsoft Visual Studio 10.0\Samples\1033\CSharpSamples\LanguageSamples\XMLdoc\XMLSample.cs" /doc:d:\XMLSample.xml
    //To see the generated XML, issue the following command: 
    //type XMLsample.xml

    //2. Gnerate class with XSD
    //c:\Program Files (x86)\Microsoft Visual Studio 10.0\VC>xsd "E:\Temp\temp.xsd" "E:\Temp\temp2.xsd" /c /outputdir:d:\
    //Writing file 'd:\Functional.cs'.

    [XmlRoot("XmlObject", Namespace = "http://www.test.com/testxmlobject/v1")]
    public class XmlObject
    {
        [XmlAttributeAttribute("schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string xsi = "http://www.test.com/test.xsd";

        [DataMember]
        [XmlElement("SOAPHeader", Namespace = "http://www.test.com/test/v1")]
        public Header Header { get; set; }

        private DateTime timeField;
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime time
        {
            get
            {
                return this.timeField;
            }
            set
            {
                this.timeField = value;
            }
        }
    }
    [XmlRoot("Header", Namespace = "http://www.test.com/testheader/v1")]
    public class Header
    {
        [DataMember]
        [XmlElement("TestID")]
        public String TestID { get; set; }
        [DataMember]
        [XmlElement("TestName")]
        public String TestName { get; set; }
    }

    public class TestXmlHelper
    {
        static void Main(string[] args)
        {

            var validator = new XsdValidator();
            validator.AddSchema(@"D:\TEMP\files\admin.xsd");
            validator.AddSchema(@"D:\TEMP\files\employees.xsd");
            var isValid = validator.IsValid(@"D:\TEMP\files\employees.xml");

            return;
        }

         public TestXmlHelper()
        {

            


            XmlObject xmlObj = new XmlObject();
            string testXmlStr = XmlHelper.ObjToXmlStr(xmlObj);

            // ObjToXmlFile
            XmlSerializer xmlser = new XmlSerializer(typeof(XmlObject));
            string path = "D:\temp\test.txt";
            FileStream fs2 = new FileStream(path, FileMode.Create, FileAccess.Write);
            xmlser.Serialize(fs2, xmlser);
             fs2.Close();

             string path3 = @"D:\temp\test.xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path3);
            String[] paras3 = { "-server", "tcp://localhost:7222", "-queue", "queue1", "-user", "admin", "-password", "123", xmlDoc.InnerXml };
            MsgProducer generatedAux3 = new MsgProducer(paras3,null,string.Empty);

            // XmlFileToObj
            XmlDocument xmlDoc2 = new XmlDocument();
            xmlDoc2.Load(path3);
            using (StringReader sr = new StringReader(xmlDoc2.InnerXml))
            {
                XmlSerializer xmldes = new XmlSerializer(typeof(XmlObject));
                XmlObject req = (XmlObject)xmldes.Deserialize(sr);
            }
        }
    }
}
