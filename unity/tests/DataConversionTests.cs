using System;
using System.Collections;
using NUnit.Framework;
using NMock2;

using DC = Roar.implementation.DataConversion;
using Roar;

[TestFixture]
public class DataConversionTests
{
	XMLNode n;
	DC.XmlToTaskHashtable ee;
	
	NMock2.Mockery mocks;

	[SetUp]
	public void SetUp()
	{
		string xml = "<task ikey=\"convert_premium_to_ingame\" >\n" +
		             "  <label>Convert Premium</label>\n" +
		             "  <description>A funky description</decsription>\n" +
		             "  <location/>\n" +
		             "  <tags/>\n" +
		             "  <costs>\n" +
		             "    <stat_cost type=\"currency\" ikey=\"premium_web\" value=\"10\" ok=\"false\" reason=\"Insufficient Premium (Web)\" />\n" +
		             "  </costs>\n" +
		             "  <rewards>\n" +
		             "    <grant_stat_range type=\"currency\" ikey=\"gamecoins\" min=\"100\" max=\"100\" />\n" +
		             "  </rewards>\n" +
		             "  <mastery level=\"\" progress=\"\"/>\n" +
		             "</task>";

		XMLNode nn = (new XMLNode.XMLParser()).Parse(xml);

		n = nn.GetFirstChild("task") as XMLNode;
        
		ee = new DC.XmlToTaskHashtable();
		mocks = new NMock2.Mockery();
	}

	[Test]
	public void CheckKey()
	{
		Assert.AreEqual("convert_premium_to_ingame", ee.GetKey(n) );
	}

	[Test]
	public void CheckLabel()
	{
		Hashtable h = ee.BuildHashtable(n);
		Assert.AreEqual( "Convert Premium", h["label"] as string);
	}

	[Test]
	public void CheckDescription()
	{
		Hashtable h = ee.BuildHashtable(n);
		Assert.AreEqual( "A funky description", h["description"] as string);
	}
	
	[Test]
	public void CheckIkey()
	{
		n = (new XMLNode("tasks","") )
			.AddAttribute("ikey","an_ikey")
			;
		Hashtable h = ee.BuildHashtable(n);
		
		Assert.AreEqual( "an_ikey", h["ikey"] );
	}

	[Test]
	public void CheckListOfCosts()
	{
		n = (new XMLNode("tasks","") )
			.AddAttribute("ikey","an_ikey")
			.AddChild(
				(new XMLNode("costs",""))
					.AddChild( new XMLNode("cost_a","") )
			        .AddChild( new XMLNode("cost_b","") )
			 );
			                                              
		Hashtable h = ee.BuildHashtable(n);
		ArrayList costs = h["costs"] as ArrayList;
		Assert.IsNotNull( costs );
		Assert.AreEqual( 2, costs.Count );
		
		Assert.IsInstanceOfType(typeof(Hashtable), costs[0] );
		Assert.IsInstanceOfType(typeof(Hashtable), costs[1] );
		
		//TODO: Add checks for the content of the costs nodes
	}
	
	[Test]
	public void CheckCRMParserCalledForCosts()
	{
		n = (new XMLNode("tasks","") )
			.AddAttribute("ikey","an_ikey")
			.AddChild(
				(new XMLNode("costs",""))
					.AddChild( new XMLNode("cost_a","") )
			        .AddChild( new XMLNode("cost_b","") )
			 );
		
		ArrayList cost_list = new ArrayList();
		ee.CrmParser_ = mocks.NewMock<DC.ICRMParser>();
		Expect.Once.On(ee.CrmParser_)
			.Method("ParseCostList")
			.With( n.GetFirstChild("costs") )
			.Will(Return.Value( cost_list ) );
		
		Hashtable h = ee.BuildHashtable(n);
		Assert.AreSame( cost_list, h["costs"] );
		
	}
	
	[Test]
	public void CheckCostParserGetsNodeNames()
	{
		n = (new XMLNode("costs","") )
			.AddChild( (new XMLNode("cost_a","")) )
			.AddChild( (new XMLNode("cost_b","")) );
		ArrayList costs = (new DC.CRMParser()).ParseCostList(n);
		Assert.AreEqual(2, costs.Count );
		Assert.AreEqual("cost_a", ((Hashtable)costs[0])["name"] );
		Assert.AreEqual("cost_b", ((Hashtable)costs[1])["name"] );
	}

}
