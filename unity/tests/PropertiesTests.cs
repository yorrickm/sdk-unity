using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NMock2;
using Roar.Components;

/**
 * Test cases for the Properties component.
 **/
[TestFixture]
public class PropertiesTests : ComponentTests
{
  protected IProperties properties;

  public static string userView = 
    @"<roar tick='128455475133'>
<user>
  <view status='ok'>
    <attribute ikey='id' value='2059428086' type='special'/>
    <attribute ikey='xp' value='0' type='special'/>
    <attribute ikey='level' value='1' type='special'/>
    <attribute ikey='facebook_uid' value='0' type='special'/>
    <attribute ikey='name' value='foo' type='special'/>
    <attribute ikey='attack' value='10' type='core' label='Attack'/>
    <attribute ikey='defence' value='10' type='core' label='Core Defence'/>
    <attribute ikey='hit' value='10' type='core' lable='Hit Power'/>
    <attribute ikey='avoid' value='10' type='core' label='avoid'/>
    <attribute ikey='health' value='100' type='resource' max='123' min='0' regen_every='1000' label='Health'/>
    <attribute ikey='energy' value='20' type='resource' max='123' min='0' regen_every='1000' label='Energy'/>
    <attribute ikey='stamina' value='5' type='resource' max='123' min='0' regen_every='1000' label='Stamina'/>
    <attribute ikey='profile_points' value='0' type='currency' label='Monkey Power Points'/>
    <attribute ikey='cash' value='100' type='currency' lable='cash'/>
    <attribute ikey='premium_currency' value='5' type='currency' label='Bear Dollars'/>
    <regen_script>
      <entry function='hello' next='1234'/>
    </regen_script>
  </view>
</user>
</roar>";
  
  [SetUp]
  new public void TestInitialise()
  {
    base.TestInitialise();
    properties = roar.Properties;
    Assert.IsNotNull(properties);
    Assert.IsFalse(properties.hasDataFromServer);
  }
  
  protected void mockFetch(string mockResponse, Roar.Callback cb) {
    requestSender.addMockResponse("user/view", mockResponse);
    properties.fetch(cb);
  }
  
  [Test]
  public void testFetchSuccess() {
    bool callbackExecuted = false;
    Roar.Callback roarCallback = (Roar.CallbackInfo callbackInfo) => { 
      callbackExecuted=true;
      Assert.AreEqual(IWebAPI.OK, callbackInfo.code);
      Assert.IsNotNull(callbackInfo.d);
    };
    mockFetch(userView, roarCallback);
    Assert.IsTrue(callbackExecuted);
    Assert.IsTrue(properties.hasDataFromServer);
  }

  [Test]
  [Ignore]
  public void testFetchFailureServerDown() {
    //assertions:
    //callback called with expected error code
    //hasDataFromServer == false
  }
  
  [Test]
  [Ignore]
  // TODO: get this test passing
  //       it appears an xml response with 0 properties will break the fetch...
  public void testListEmpty() {
    //returns an empty array prior to fetch called
    Assert.AreEqual(0, properties.list().Count);

    //returns an empty array if no properties available
    mockFetch(@"<roar tick='128455475133'>
<user>
  <view status='ok'>
  </view>
</user>
</roar>", null);
    Assert.IsTrue(properties.hasDataFromServer);
    Assert.AreEqual(0, properties.list().Count);
  }

  [Test]
  public void testList() {

    mockFetch(userView, null);
    Assert.IsTrue(properties.hasDataFromServer);
    
    //returns a list of properties with the expected data structure
    int expectedPropertyCount = 15;
    ArrayList propertyHashtables = properties.list();
    Assert.AreEqual(expectedPropertyCount, propertyHashtables.Count);
    
    //invokes callback with parameter *data* containing the list of Hashtable shop items
    bool callbackExecuted = false;
    Roar.Callback roarCallback = (Roar.CallbackInfo callbackInfo) => { 
      callbackExecuted=true;
      Assert.AreEqual(IWebAPI.OK, callbackInfo.code);
      Assert.IsNotNull(callbackInfo.d);
      Assert.AreEqual(callbackInfo.d, propertyHashtables);
    };
    propertyHashtables = properties.list(roarCallback);
    Assert.IsTrue(callbackExecuted);
    Assert.AreEqual(expectedPropertyCount, propertyHashtables.Count);
  }

  [Test]
  public void testGetProperty() {
    
    //returns null on no data from server
    Assert.IsNull(properties.getProperty("stamina"));
    
    mockFetch(userView, null);
    
    //returns Hashtable of property if exists
    Hashtable staminaProperty = properties.getProperty("stamina") as Hashtable;
    StringAssert.IsMatch("5", staminaProperty["value"] as String);
    StringAssert.IsMatch("resource", staminaProperty["type"] as String);
    StringAssert.IsMatch("123", staminaProperty["max"] as String);
    StringAssert.IsMatch("0", staminaProperty["min"] as String);
    StringAssert.IsMatch("1000", staminaProperty["regen_every"] as String);
    StringAssert.IsMatch("Stamina", staminaProperty["label"] as String);

    //returns null on property not existing
    Assert.IsNull(properties.getProperty("doesnotexist"));
  }

  [Test]
  public void testGetValue() {
    
    //returns null on no data from server
    Assert.IsNull(properties.getValue("stamina"));
    
    mockFetch(userView, null);
    
    //returns string of property value attribute if exists
    StringAssert.IsMatch("5", properties.getValue("stamina") as String);
    
    //returns null on property not existing
    Assert.IsNull(properties.getProperty("doesnotexist"));
  }
  
  [Test]
  public void testPropertyUpdated() {
    
    mockFetch(userView, null);
    
    StringAssert.IsMatch("5", properties.getValue("stamina") as String);
    
    IXMLNode updateNode = IXMLNodeFactory.instance.Create("<update type='attribute' ikey='stamina' value='2'/>").GetFirstChild("update");
    
    RoarIOManager.OnRoarServerUpdate(updateNode);
    
    StringAssert.IsMatch("2", properties.getValue("stamina") as String);
    
  }
}

