using System;


public class ComponentTests
{
  protected MockRoarIO roar;
  protected IRoarIO roarApi;
  protected MockRequestSender requestSender;
  
  public void TestInitialise() {
    IXMLNodeFactory.instance = new SystemXMLNodeFactory();
    roar = new MockRoarIO();
    roarApi = roar as IRoarIO;
    roar.Awake();
    requestSender = roar.api as MockRequestSender;
  }
}

