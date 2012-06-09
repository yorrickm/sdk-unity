using System;

public class MockRoarIO : RoarIO, IRoarIO
{
  public RequestSender api;
  
  new public void Awake ()
  {
    Config_ = new Roar.implementation.Config ();
    Logger logger = new Logger ();

    api = new MockRequestSender (Config_, this, logger);
    Roar.implementation.DataStore data_store = new Roar.implementation.DataStore (api, logger);
    WebAPI_ = new global::WebAPI (this, api);
    User_ = new Roar.implementation.Components.User (WebAPI_.user, data_store, logger);
    Properties_ = new Roar.implementation.Components.Properties (data_store);
    Inventory_ = new Roar.implementation.Components.Inventory (WebAPI_.items, data_store, logger);
    Data_ = new Roar.implementation.Components.Data (WebAPI_.user, data_store, logger);
    Shop_ = new Roar.implementation.Components.Shop (WebAPI_.shop, data_store, logger);
    Actions_ = new Roar.implementation.Components.Actions (WebAPI_.tasks, data_store);

    UrbanAirship_ = new Roar.implementation.Adapters.UrbanAirship (WebAPI_);

    // Apply public settings 
    Config.game = gameKey;
  }
}

