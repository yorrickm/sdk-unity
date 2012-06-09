using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NMock2;

[TestFixture]
public class ActionsTests
{
  [SetUp]
  public void TestInitialise()
  {
  }
  
  [Test]
  [Ignore]
  public void testFetchSuccess() {
    //Assert.Fail();
    //assertions:
    //callback called with data equalling hash table containing actions available to user
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
    //returns an empty array if no actions available
    //returns a list of actions with the expected data structure
    //invokes callback with parameter *data* containing the list of Hashtable actions
  }

  // ambiguous tests…
  [Test]
  [Ignore]
  public void testExecuteFailure() {
    //assertions:
    //invokes callback with error code and error message
  }
 
  [Test]
  [Ignore]
  public void testExecute() {
    //assertions:
    //invokes callback with data param containing result of executed action 
  }
}

