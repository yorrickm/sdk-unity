using System;


public class ComponentTests
{
  protected MockRoar roar;
  protected IRoar roarApi;
  protected MockRequestSender requestSender;
  
  public void TestInitialise() {
    IXMLNodeFactory.instance = new SystemXMLNodeFactory();
    roar = new MockRoar();
    roarApi = roar as IRoar;
    roar.Awake();
    requestSender = roar.api as MockRequestSender;
  }
}

