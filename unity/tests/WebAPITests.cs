using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NMock2;
using System.Reflection;


[TestFixture()]
public class WebAPITests
{
	private Mockery mockery = null;

	
    [SetUp]
    public void TestInitialise()
    {
        this.mockery = new Mockery();
    }
	
	[Test()]
	public void testNotifyOfServerChanges()
	{
		IXMLNode serverNode = this.mockery.NewMock<IXMLNode>();
		
		IXMLNode[] xml_node_array = { };
		IEnumerable<IXMLNode> an_enumerable = xml_node_array;
		
		Expect.Once.On(serverNode).GetProperty("Children").Will( Return.Value( an_enumerable ) );
		
		bool called=false;

		Action<object> callback = o => called=true;

		RoarManager.roarServerAllEvent += callback;
		
		RoarManager.NotifyOfServerChanges( serverNode );

		Assert.IsTrue(called);
		
		this.mockery.VerifyAllExpectationsHaveBeenMet();
	}
}

