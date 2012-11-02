using System.Xml;

public class SystemXMLNodeFactory : IXMLNodeFactory
{

	public override IXMLNode Create()
	{
		return new XMLNode();
	}

	public override IXMLNode Create(string xml)
	{
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(xml);
		return new SystemXMLNode(doc);
	}

	public override IXMLNode Create(string name, string text)
	{
		XmlDocument doc = new XmlDocument();
		XmlElement element = doc.CreateElement(name);
		doc.AppendChild(element);
		element.AppendChild(doc.CreateTextNode(text));
		return new SystemXMLNode(doc);
	}
}
