using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NMock2;

[TestFixture]
public class InventoryTests
{
  [SetUp]
  public void TestInitialise()
  {
  }
  
  [Test]
  [Ignore]
  public void testFetchSuccess() {
    //assertions:
    //callback called with data equalling hash table containing shop items available to user
    //hasDataFromServer == true
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
  public void testList() {
    //assertions:
    //returns an empty array prior to fetch called
    //returns an empty array if no shop items available
    //returns a list of shop items with the expected data structure
    //invokes callback with parameter *data* containing the list of Hashtable shop items
  }
          
  [Test]
  [Ignore]
  public void testHas() {
    //assertions:
    //returns true if one or more of item type
    //returns false if no data from server  
  }

  [Test]
  [Ignore]
  public void testQuantity() {
    //returns 0 on none of item type or no data from server
  }
  
  [Test]
  [Ignore]
  public void testGetGood() {
    //assertions:
    //returns Hashtable of item if exists
    //returns null on good not existing or no data from server
  }
  
  [Test]
  [Ignore]
  public void testSell() {
    //assertions:
    //correct event triggered with expected arguments (GoodInfo)
    //callback executed with expected arguments (GoodInfo)
  }
  
  [Test]
  [Ignore]
  public void testUse() {
    //assertions:
    //correct event triggered with expected arguments (GoodInfo)
    //callback executed with expected arguments (GoodInfo)
  }

  [Test]
  [Ignore]
  public void testActivate() {
    //assertions:
    //correct event triggered with expected arguments (GoodInfo)
    //callback executed with expected arguments (GoodInfo)
  }
  
  [Test]
  [Ignore]
  public void testDeactivate() {
    //assertions:
    //correct event triggered with expected arguments (GoodInfo)
    //callback executed with expected arguments (GoodInfo)
  }
 
  [Test]
  [Ignore]
  public void testSellFailureServerDown() {
    //assertions:
    //callback called with expected error code    
  }

  [Test]
  [Ignore]
  public void testUseFailureServerDown() {
    //assertions:
    //callback called with expected error code    
  }

  [Test]
  [Ignore]
  public void testActivateFailureServerDown() {
    //assertions:
    //callback called with expected error code    
  }

  [Test]
  [Ignore]
  public void testDeactivateFailureServerDown() {
    //assertions:
    //callback called with expected error code    
  }
  
}

