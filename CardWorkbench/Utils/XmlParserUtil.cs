using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CardWorkbench.Utils
{
    class XmlParserUtil
    {
        public static XmlDocument xmlDocument;

        //获得xmlDocument初始化对象实例
        public static XmlDocument getXmlDocumnetInstance(string path) {
            if (xmlDocument == null)
	    {
            xmlDocument = new XmlDocument();
		 
	    }
            xmlDocument.Load(path);
            //xmlDocument.LoadXml(xmlStr);
            return xmlDocument;
        }

        //根据节点名获取xml节点对象
        public static XmlNode getXmlSingleNodeByName(string nodeName) {
            return xmlDocument.SelectSingleNode(nodeName);
        }

        //根据唯一的节点名获取xml所有的子节点对象
        public static XmlNodeList getChildNodesListBySingleParentNodeName(string nodeName)
        {
            return xmlDocument.SelectSingleNode(nodeName).ChildNodes;           
        }

        //根据xml节点对象和属性名获取对应的节点属性值
        public static string getNodeAttributeValue(XmlNode xmlNode, string attributeName)
        {
            XmlAttributeCollection xmlAttrCollection = xmlNode.Attributes;
            foreach (XmlAttribute xmlAttr in xmlAttrCollection)
            {
                string attrValue = xmlAttr.Value;
                if (attributeName.Equals(xmlAttr.Name))
                {
                    return attrValue;
                }
            }
            return "";
        }

        //根据xml节点名称在所有节点列表中查询返回该节点的值
        public static string getNodeInnerText(XmlNode xmlNode)
        {
            if (xmlNode != null)
            {
            return xmlNode.InnerText;
                
            }
            return "";
           
        }

    }
}
