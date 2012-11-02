using System;
using System.Collections;
using NUnit.Framework;

public class MockRequestSender : RequestSender, IRequestSender
{
  protected Hashtable responses;
  
  public MockRequestSender (Roar.IConfig config, IUnityObject unity_object, Roar.ILogger logger) : base(config, unity_object, logger)
  {
    responses = new Hashtable();
  }
  
  new public void make_call( string apicall, Hashtable args, IRequestCallback<IXMLNode> cb ) {
    Assert.IsTrue(responses.ContainsKey(apicall), "no mock response setup for api path '" + apicall + "'");
    onServerResponse(responses[apicall] as String, apicall, cb);
  }
  
  public void addMockResponse(string apicall, string rawResponse) {
    responses[apicall] = rawResponse;
  }
  
  public void reset() {
    responses.Clear();
  }
}
