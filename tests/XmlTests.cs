using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.IO;
using System.Text;

[TestFixture]
public class XmlParserTest
{
	public List<IXMLNodeFactory> xmlNodeFactories;
	
	public EnumerableCounter<KeyValuePair<string, string>> AttributeCounter;
	public EnumerableCounter<IXMLNode> ChildrenCounter;
	
	/** Keeps track of how many nodes are processed by recursive AssertXMLNodeSane().**/
    private static int nodesProcessed = 0;
	
	/**
	 * Pulls a test file from the tests/data folder. 
	 **/
	private String loadTestData(string filename) {
		string filePath = "./data/" + filename;
		return System.IO.File.ReadAllText(filePath, Encoding.UTF8);
	}
	
	/**
	 * Returns the number of items in an IEnumerable, there must be a smarter c# way of doing this.
	 **/
	public class EnumerableCounter<T> {
		public EnumerableCounter() {
		}
		public int Count(IEnumerable<T> i) {
			int c = 0;
			foreach(T t in i) {
				++c;
			}
			return c;	
		}
	}

	[SetUp]
	public void SetUp()
	{
		xmlNodeFactories = new List<IXMLNodeFactory>() {
			new XMLNodeFactory(), 
			new SystemXMLNodeFactory()
		};
		AttributeCounter = new EnumerableCounter<KeyValuePair<string, string>>();
		ChildrenCounter = new EnumerableCounter<IXMLNode>();
	}
	
	[Test]
	public void TestFluentConstructorAndAsString()
	{
		XMLNode test = 
	    (new XMLNode("task", ""))
		.AddAttribute( "ikey", "convert_premium_to_ingame" )
		.AddChild(
			(new XMLNode("label", "Convert Premium"))
		)
		.AddChild(
			(new XMLNode("description", "A funky description"))
		)
		;
		
		Assert.AreEqual(
			"(new XMLNode(\"task\", \"\"))\n" +
            "  .AddAttribute( \"ikey\", \"convert_premium_to_ingame\" )\n" +			
            "  .AddChild(\n" + 
            "    (new XMLNode(\"label\", \"Convert Premium\"))\n" +
            "  )\n" +
			"  .AddChild(\n" + 
			"    (new XMLNode(\"description\", \"A funky description\"))\n" +
            "  )\n",
			test.ShowAsBuildString("") );

	}

	[Test]
	public void testXMLNodeParserShouldWork() 
	{
		testParserShouldWork(new XMLNodeFactory());
	}
	
	[Test]
	public void testSystemXMLNodeParserShouldWork() 
	{
		testParserShouldWork(new SystemXMLNodeFactory());
	}
	
	protected void testParserShouldWork(IXMLNodeFactory xmlNodeFactory) 
	{
		IXMLNode node = xmlNodeFactory.Create("<example><value type=\"String\">Foobar</value><value type=\"Int\">3</value></example>");
			
		Assert.IsNotNull(node);

		//Assert.IsNull( node.Name );
		//Assert.IsNull( node.Text );
		
		List<KeyValuePair<string,string>> Attributes = new List<KeyValuePair<string, string>>(node.Attributes);

		Assert.IsNotNull( Attributes );
		Assert.AreEqual(0, Attributes.Count );

		List<IXMLNode> Children = new List<IXMLNode>(node.Children);
		Assert.IsNotNull( Children );
		Assert.AreEqual( 1, Children.Count, "Incorrect number of members in root node");

		//Check the element node.
		IXMLNode e = Children[0] as IXMLNode;
		Assert.IsNotNull( e, "Expected non-null node" );
		Assert.AreEqual("example", e.Name );
		Assert.AreEqual("", e.Text );

		List<IXMLNode> eChildren = new List<IXMLNode>(e.Children);
		Assert.IsNotNull( eChildren );
		Assert.AreEqual( 2, eChildren.Count ); //There's 2 nodes, but they're both value nodes!
		
		List<KeyValuePair<string,string>> eAttributes = new List<KeyValuePair<string, string>>(e.Attributes);

		Assert.IsNotNull( eAttributes );
		Assert.AreEqual(0, eAttributes.Count );


		//Check the first value node
		IXMLNode v1 = eChildren[0] as IXMLNode;
		Assert.IsNotNull(v1);
		Assert.AreEqual("value", v1.Name);
		Assert.AreEqual("Foobar", v1.Text);

		Assert.AreEqual(0, ChildrenCounter.Count(v1.Children) );
		Assert.AreEqual(1, AttributeCounter.Count(v1.Attributes) );
		Assert.AreEqual("String", v1.GetAttribute("type") );

		//Check the second value node
		IXMLNode v2 = eChildren[1] as IXMLNode;
		Assert.IsNotNull(v2);
		Assert.AreEqual("value", v2.Name);
		Assert.AreEqual("3", v2.Text);

		Assert.AreEqual(0, ChildrenCounter.Count(v2.Children) );
		Assert.AreEqual(1, AttributeCounter.Count(v2.Attributes) );
		Assert.AreEqual("Int", v2.GetAttribute("type") );
	}

	[Test]
	public void testXMLNodeGetNodeList() 
	{
		testIXMLNodeGetNodeList(new XMLNodeFactory());
	}
	
	[Test]
	public void testSystemXMLNodeGetNodeList() 
	{
		testIXMLNodeGetNodeList(new SystemXMLNodeFactory());
	}
	
	protected void testIXMLNodeGetNodeList(IXMLNodeFactory xmlNodeFactory) 
	{
		IXMLNode root = xmlNodeFactory.Create("<example><value type=\"String\">Foobar</value><value type=\"Int\">3</value></example>");
	
		// test XMLNode.GetNodeList
		List<IXMLNode> actualNodeList = root.GetNodeList("example>0>value");
		Assert.AreEqual( 2, actualNodeList.Count );
	}

	[Test]
	public void testXMLNodeGetNode() 
	{
		testIXMLNodeGetNode(new XMLNodeFactory());
	}
	
	[Test]
	public void testSystemXMLNodeGetNode() 
	{
		testIXMLNodeGetNode(new SystemXMLNodeFactory());
	}
	
	protected void testIXMLNodeGetNode(IXMLNodeFactory xmlNodeFactory)
	{
		IXMLNode node = xmlNodeFactory.Create("<example><value type=\"String\">Foobar</value><value type=\"Int\">3</value></example>");

		// test XMLNode.GetNode
		IXMLNode actualNode = node.GetNode("example>0>value>1");

		Assert.AreEqual("3", actualNode.Text );
	}
	
	[Test]
	public void testXMLNodeGetValue() 
	{
		testIXMLNodeGetValue(new XMLNodeFactory());
	}
	
	[Test]
	public void testSystemXMLNodeGetValue() 
	{
		testIXMLNodeGetValue(new SystemXMLNodeFactory());
	}

	protected void testIXMLNodeGetValue(IXMLNodeFactory xmlNodeFactory)
	{
		IXMLNode root = xmlNodeFactory.Create("<example><value type=\"String\">Foobar</value><value type=\"Int\">3</value></example>");
		string actualValue = root.GetValue("example>0>value>1>type");
		Assert.AreEqual("Int", actualValue );
	}
	 
	/**
	 * Tests that the expected number of nodes exist in the parsed tree.
	 * Tests that every node in the parsed xml:
	 * 1. Has a child dictionary.
	 * 2. Has an attributes dictionary.
	 * 3. Has a non-null text field.
	 * 4. Has a non-blank name.
	 **/
	[Test]
	public void testXmlTreeSanity() {
		string raw = loadTestData("good_response_1.xml");
		XMLNode root = new XMLNode.XMLParser().Parse(raw);
		nodesProcessed = 0;
		int expectedNodesProcessed = 30;
		// TODO: it appears the the root node has no name... perhaps the XMLParser should return the <roar> node 
		// as root instead of creating a bogus un-named container node that causes our node sanity test to fail?
		// or the automatic container node could be called <xml>...
		// AssertXMLNodeSane(root);
		AssertXMLNodeSane(root.GetNode("roar>0") as XMLNode);
		Assert.AreEqual(expectedNodesProcessed, nodesProcessed);
	}
	
	/**
	 * .
	 **/
	[Test]	
	public void testXMLNodeRoarXml() {
		testRoarXml(new XMLNodeFactory());
	}
	
	/**
	 * .
	 **/
	[Test]	
	public void testSystemXMLNodeRoarXml() {
		testRoarXml(new SystemXMLNodeFactory());
	}

	/**
	 * Uses a mashed-up Roar.io XML response to test the xml parser.  
	 **/
	protected void testRoarXml(IXMLNodeFactory xmlNodeFactory)
	{		
		string raw = loadTestData("good_response_1.xml");
		IXMLNode root = xmlNodeFactory.Create(raw);
		Assert.IsNotNull(root);
		
		// test roar node
		IXMLNode roar = root.GetNode("roar>0");
		Assert.IsNotNull(roar);
		Assert.AreEqual(1, AttributeCounter.Count(roar.Attributes) );
		Assert.AreEqual(2, ChildrenCounter.Count(roar.Children) );
		Assert.AreEqual("133591623162", roar.GetAttribute("tick") );
	
		// test the task node
		
		// <task><mastery> has attributes with no value: <mastery level="" progress=""/>
		// make sure they are parsed as empty strings
		IXMLNode mastery = root.GetNode("roar>0>tasks>0>list>0>task>0>mastery>0");
		Assert.IsNotNull(mastery);
		Assert.AreEqual(2, AttributeCounter.Count(mastery.Attributes) );
		Assert.AreEqual("", mastery.GetAttribute("level") );
		Assert.AreEqual("", mastery.GetAttribute("progress") );
		Assert.AreEqual("", mastery.GetValue("level") );
		Assert.AreEqual("", mastery.GetValue("progress") );
		
		// test list node
		// <list><task><label> should have a text value
		IXMLNode taskLabel = root.GetNode("roar>0>tasks>0>list>0>task>0>label>0");
		Assert.IsNotNull(taskLabel);
		Assert.AreEqual("label", taskLabel.Name);
		Assert.AreEqual("Monolith Unlock", taskLabel.Text);
		
		// test parsing <server> node - child of root node
		IXMLNode server = root.GetNode("roar>0>server>0");
		Assert.IsNotNull(server);
		Assert.AreEqual(0, AttributeCounter.Count(server.Attributes) );
		Assert.AreEqual("133591623162", roar.GetAttribute("tick") );
		
		// test parsing of 2 <update> nodes in the server node
		// <update type="attribute" ikey="intelligence" value="60"/>
	    // <update type="attribute" ikey="agility" value="30"/>
		List<IXMLNode> updates = server.GetNodeList("update");
		Assert.IsNotNull(updates);
		Assert.AreEqual(2, updates.Count);
		IXMLNode intelligenceUpdate = updates[0];
		IXMLNode agilityUpdate = updates[1];
		
		// each <update> node should have 3 attributes
		Assert.AreEqual(3, AttributeCounter.Count(intelligenceUpdate.Attributes));
		Assert.AreEqual(3, AttributeCounter.Count(agilityUpdate.Attributes));
		
		// there shouldn't be any children for the <update> nodes
		Assert.AreEqual(0, ChildrenCounter.Count(intelligenceUpdate.Children));
		Assert.AreEqual(0, ChildrenCounter.Count(agilityUpdate.Children));
		
		// get the attributes for the intelligence <update> using XMLNode.Attributes()
		Assert.AreEqual("attribute", intelligenceUpdate.GetAttribute("type"));
		Assert.AreEqual("intelligence", intelligenceUpdate.GetAttribute("ikey"));
		Assert.AreEqual("60", intelligenceUpdate.GetAttribute("value"));
		Assert.AreEqual(0, ChildrenCounter.Count(intelligenceUpdate.Children));
		
		// this time get the attributes for the agility <update> using XMLNode.GetValue()
		Assert.AreEqual("attribute", agilityUpdate.GetValue("type"));
		Assert.AreEqual("agility", agilityUpdate.GetValue("ikey"));
		Assert.AreEqual("30", agilityUpdate.GetValue("value"));
	}
	

	/**
	 * Tests that the XMLNode can unescape xml entities.  
	 **/
	[Test]	
	public void testXMLNodeEntitiesUnescapedDuringParse() {
		testIXMLNodeEntitiesUnescapedDuringParse(new XMLNodeFactory());
	}
	
	/**
	 * Tests that the SystemXMLNode can unescape xml entities.  
	 **/
	[Test]	
	public void testSystemXMLNodeEntitiesUnescapedDuringParse() {
		testIXMLNodeEntitiesUnescapedDuringParse(new SystemXMLNodeFactory());
	}
	
	/**
	 * Tests that an IXMLNode implementation can unescape xml entities.
	 **/
	protected void testIXMLNodeEntitiesUnescapedDuringParse(IXMLNodeFactory xmlNodeFactory) {
		string raw = loadTestData("escaped.xml");
		IXMLNode root = xmlNodeFactory.Create(raw);
		Assert.IsNotNull(root);
		
		IXMLNode node = root.GetNode("roar>0>message>0");
		Assert.IsNotNull(node);
		
		string expectedNodeValue = "<log level=\"warn\">'Warning&#x21;'</log>";
		string actualNodeValue = node.Text;
		
		Assert.AreEqual(expectedNodeValue, actualNodeValue);
	}
	
	/**
	 * Tests that the XMLNode can handle UTF-8 simplified chinese characters.  
	 **/
	[Test]	
	public void testXMLNodeUTF8Support() {
		testIXMLNodeUTF8Support(new XMLNodeFactory());
	}
	
	/**
	 * Tests that the SystemXMLNode can handle UTF-8 simplified chinese characters.  
	 **/
	[Test]	
	public void testSystemXMLNodeUTF8Support() {
		testIXMLNodeUTF8Support(new SystemXMLNodeFactory());
	}
	
	/**
	 * Tests that an IXMLNode implementation can handle UTF-8 simplified chinese characters.
	 **/
	protected void testIXMLNodeUTF8Support(IXMLNodeFactory xmlNodeFactory)
	{
		string raw = loadTestData("simplified_chinese.xml");
		IXMLNode root = xmlNodeFactory.Create(raw);
		Assert.IsNotNull(root);
		
		IXMLNode node = root.GetNode("roar>0>message>0");
		Assert.IsNotNull(node);
		
		string actualAttributeValue = node.GetAttribute("ikey");
		string actualNodeValue = node.Text;
		
		string expectedAttributeValue = "你好";
		string expectedNodeValue = "吼声引擎";
		
		Assert.AreEqual(expectedAttributeValue, actualAttributeValue);
		Assert.AreEqual(expectedNodeValue, actualNodeValue);
	}
	
	/**
	 * XMLNode tree traversal function that asserts each node in the tree is sane.
	 **/
	private static void AssertXMLNodeSane(XMLNode xmlNode) {
		++nodesProcessed;
		Assert.IsNotNull(xmlNode, "AssertXMLNodeSane() passed a null XMLNode!");
		Assert.IsNotNull(xmlNode.Children, "XMLNode children dictionary shouln't be null!");
		Assert.IsNotNull(xmlNode.Attributes, "XMLNode attributes dictionary shouln't be null!");
		Assert.IsNotNull(xmlNode.Text, "XMLNode text shouldn't be null!");
		Assert.IsNotEmpty(xmlNode.Name, "XMLNode name shouldn't be blank!");
		
		// the current node is good - let's test the children of this node
		foreach( XMLNode n in xmlNode.Children) {
				AssertXMLNodeSane(n);
		}
	}
}
