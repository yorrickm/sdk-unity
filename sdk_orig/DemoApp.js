#pragma strict

var game_name = "boomboxer";
var user_name = "bobo";
var password  = "bobo";

Debug.Log('MyDemoApp!-----');

// Set this to use a real facebook app id and 
// you'll need to replace the Facebook implementation with a real one
// such as the social networking plugin from prime31.
IXMLNodeFactory.instance = new XMLNodeFactory();

var roar_:IRoar;
function Awake()
{
  Debug.Log("*** DemoApp.js Awakening ***");
  roar_ = GetComponent(DefaultRoar) as IRoar;
}

function Start()
{
  Debug.Log("*** DemoApp.js Starting ***");
  FacebookBinding.init("123123_AN_APP_ID_12312");
  roar_.Config.game = game_name;
  Debug.Log(roar_);
}




function onLogin()
{
  Debug.Log('--Logged in with authtoken '+roar_.AuthToken );
  uiState = UIState.Started;
  roar_.Properties.fetch(null);
}
RoarManager.loggedInEvent += onLogin;

function onLoginFailed( mesg:String )
{
  Debug.Log('--Login failed :' + mesg);
  
  uiState = UIState.Error;
  uiErrorMessage = mesg;
}
RoarManager.logInFailedEvent += onLoginFailed;
RoarManager.createUserFailedEvent += onLoginFailed;

function onFacebookLogin()
{
  var oauth_token:String = FacebookBinding.getAccessToken();
  roar_.login_facebook_oauth( oauth_token, function(d:Roar.CallbackInfo){});
}
FacebookManager.loginSucceededEvent += onFacebookLogin;


var gotProps:boolean = false;
RoarManager.propertiesReadyEvent += function(){ gotProps=true; };

var gotShop:boolean = false;
RoarManager.shopReadyEvent += function(){ gotShop=true; };

var flagShowShop:boolean = false;
var scroll:Vector2 = Vector2.zero;
public var ss:GUIStyle = new GUIStyle();

// The variable to control where the scrollview 'looks' into its child elements.
var scrollPosition : Vector2;

enum UIState {
  Start,
  LoadingPlayer,
  Started,
  Shop,
  Inventory,
  Events,
  Error
  };

var uiState : UIState = UIState.Start;
var uiErrorMessage : String = "";



//Here's what an incomming CRM List looks like
//[
//  {
//    grant_stat_range: [
//    {
//        ikey: "gamecoins" : System.String
//        _text: "" : System.String
//        type: "currency" : System.String
//        min: "100" : System.String
//        _name: "grant_stat_range" : System.String
//        max: "100" : System.String
//     }
//    ]
//  }
//]
//
// It'd be much nicer to deal with if it looked like this:
// [
//   {
//     ikey: "gamecoins" : System.String
//     _text: "" : System.String
//     type: "currency" : System.String
//     min: "100" : System.String
//     _name: "grant_stat_range" : System.String
//     max: "100" : System.String
//   }
// ]

function formatModifierList( ms:ArrayList ) : String
{  
  var retval:String = "";
  for( var i:int=0; i<ms.Count; ++i)
  {
    var m = ms[i] as Hashtable;
    if(!m) continue;
    
    retval += " : ";
    if( m['name'] == "grant_stat_range" )
    {
      if(m['min']==m['max'])
      {
        retval += m['min']+" "+m['ikey'];
      }
      else
      {
        retval += m['min']+" -- "+m['max']+" "+m['ikey'];
      }
    }
    else if(m['name'] == "grant_stat" )
    {
      retval += m['value'] + " " + m['ikey'];
    }
    else
    {
      retval += m['name'] + "not yet formatted";
    }
    retval += " : ";
  }
  return retval;
}

function formatCostsList( ms:ArrayList ) : String
{
  var retval:String = "";
  for( var i:int=0; i<ms.Count; ++i)
  {

    var m = ms[i] as Hashtable;
    if(!m) continue;
    retval += " : ";
    if( m['name']=='item_cost' )
    {
      retval += m['number_required']+" "+m['ikey'];
    }
    else if( m['name']=='stat_cost' )
    {
      retval += m['value']+" "+m['ikey'];
    }
    else
    {
      retval += m['name'] + "not yet formatted";
    }
    retval += " : ";
  }
  return retval;
}

function formatRequirementList( ms:ArrayList ) : String
{
  var retval:String = "";
  for( var i:int=0; i<ms.Count; ++i)
  {

    var m = ms[i] as Hashtable;
    if(!m) continue;
    retval += " : ";
    if( m["name"]=="stat_requirement" )
    {
      retval += m['value'] + " " +m['ikey'];
    }
    else
    {
      retval += m['name'] + "not yet formatted";
    }
    retval += " : ";
  }
  return retval;
}

function ShopUI()
{
  GUILayout.Box("SHOP" );
  if (roar_.Shop.hasDataFromServer)
  {
    var shoplist = roar_.Shop.list() as ArrayList;
    for (var i:int=0; i<shoplist.Count; i++)
    {
      var shopitem = shoplist[i] as Hashtable;
      var costs = (shopitem['costs'] as ArrayList)[0] as Hashtable;
      
      GUILayout.BeginVertical( ss, GUILayout.Width(100),GUILayout.Height(100));
      GUILayout.Label( shopitem['label'] as String );
      GUILayout.Label(costs['value']+ ' '+costs['ikey'] );
      if( GUILayout.Button( 'Buy' ) )
      {
        roar_.Shop.buy( shopitem['shop_ikey'] as String, 
           function(d:Roar.CallbackInfo)
           {
             Debug.Log(Roar.Json.ObjectToJSON(d)); 
           });
      }
      GUILayout.EndVertical();
    }
  }
  else GUILayout.Label('No data yet');
}

function InventoryUI()
{
  GUILayout.Box("INVENTORY" );
  if (roar_.Inventory.hasDataFromServer)
  {
    // GUILayout.BeginHorizontal( GUILayout.MaxWidth(listW) );
    var items = roar_.Inventory.list() as ArrayList;
//    Debug.Log("inventory = " + Roar.ObjectToJSON( items ) );
    for (var i:int=0; i<items.Count; i++)
    {
      var item = items[i] as Hashtable;
      
      GUILayout.BeginVertical( ss, GUILayout.Width(100),GUILayout.Height(100));
      GUILayout.Label( item['label'] + (item['equipped']?' (equipped)':'') );
      GUILayout.Label( item['ikey'] as String );
      if( item['consumable'] )
      {
        if( GUILayout.Button("Use") )
        {
          Debug.Log("use called for "+item['id'].ToString() );
          roar_.Inventory.use( item['id'] as String,
            function(d:Roar.CallbackInfo)
            {
              Debug.Log("Called Inventory.use .. got\n" + Roar.Json.ObjectToJSON(d));
            } );
        }
      } else {
	      // TODO:  There's really some additional cases here.
	      //  1. Its equipable, but I've already got the max of that type equipped.
	      //  2. It's not equippable ever.
	      if( item['equipped'] )
	      {
		      if(GUILayout.Button("Unequip"))
		        {
		            Debug.Log("deactivate called for " + item['id'].ToString() );
		            roar_.Inventory.deactivate( item['id'] as String,
			           function(d:Roar.CallbackInfo)
			           {
			             Debug.Log("Called Inventory.deactivate .. got");
			             Debug.Log(Roar.Json.ObjectToJSON(d)); 
			           } );
		        }
		      }
		      else
		      {
		        if( GUILayout.Button("Equip") )
		        {
		            Debug.Log("activate called for " + item['id'].ToString() );
		            roar_.Inventory.activate( item['id'] as String,
			           function(d:Roar.CallbackInfo)
			           {
			             Debug.Log("Called Inventory.activate .. got");
			             Debug.Log(Roar.Json.ObjectToJSON(d)); 
			           } );
		        }
		      }
      }
      if( item['sellable'] )
      {
        if(GUILayout.Button("Sell"))
        {
          Debug.Log("sell called for "+item['id'].ToString() );
          roar_.Inventory.sell( item['id'] as String,
            function(d:Roar.CallbackInfo)
            {
              Debug.Log("Called Inventory.sell .. got\n" + Roar.Json.ObjectToJSON(d));
            } );
        }
      }
      
      
      
      GUILayout.EndVertical();
    }
    // GUILayout.EndHorizontal(); 
  }
  else GUILayout.Label('No data yet');
}

function ActionsUI()
{
  GUILayout.Box("ACTIONS");
  if (!roar_.Actions.hasDataFromServer)
  {
    GUILayout.Label('No data yet');
    return;
  }

  var actions = roar_.Actions.list() as ArrayList;
  for (var i:int=0; i<actions.Count; i++)
  {
    var action = actions[i] as Hashtable; 
    GUILayout.BeginVertical( ss, GUILayout.Width(300),GUILayout.Height(100));
    GUILayout.Label( action['ikey'] as String );
    GUILayout.Label( action['label'] as String );

    if( action['costs']!=null )
    {
      GUILayout.Label( "costs" );
      GUILayout.Label( formatCostsList( action['costs'] as ArrayList ) );
    }
    if( action['rewards']!=null )
    {
      GUILayout.Label( "rewards" );
      GUILayout.Label( formatModifierList( action['rewards'] as ArrayList ) );
    }
    if( action['requires']!=null )
    {
      GUILayout.Label( "requires" );
      if( action["requires"] as ArrayList == null )
      {
        Debug.Log("requires field was not an ArrayList, instead it was " + Roar.Json.ObjectToJSON(action["requires"]) );
        GUILayout.Label( "???" );
      }
      else
      { 
      	GUILayout.Label( formatRequirementList( action['requires'] as ArrayList ) );
      }
    }
 
    if( GUILayout.Button("Do it!") )
    {
      Debug.Log("activate called for " + action['ikey'].ToString() );
      roar_.Actions.execute( action['ikey'] as String,
        function(d:Roar.CallbackInfo)
        {
          Debug.Log("Called  Actions.execute .. got");
          Debug.Log(Roar.Json.ObjectToJSON(d));
        } );
    }

    //GUILayout.Label( action['label'] ); //Fix this -  its coming in as am empty array!
    GUILayout.EndVertical();
  }
}


function OnGUI () {
  var h = Screen.height;
  var w = Screen.width;

  // Background
  GUI.Box( Rect(10,10,100,150), "");

  if(uiState == UIState.Start)
  {
    if (GUI.Button( Rect(20,20,80,20), "Loginload" )) 
    {
      uiState = UIState.LoadingPlayer;
      roar_.login(user_name, password, function(d:Roar.CallbackInfo){});
    }

    if (GUI.Button( Rect(20,50,80,20), "Create" ))
    {
      uiState = UIState.LoadingPlayer;
      roar_.create(user_name, password, function(d:Roar.CallbackInfo){});
    }
    
    if (GUI.Button( Rect(20,80,80,20), "Login FB" ))
    {
      uiState = UIState.LoadingPlayer;
      FacebookBinding.login();
    }
    
//    if (GUI.Button( Rect(20,60,80,20), "Create FB" ))
//    {
//      uiState = UIState.LoadingPlayer;
//      gocreate();
//    }
  }



  // -- Navigation Buttons
  if( uiState != UIState.Start && uiState != UIState.LoadingPlayer && uiState != UIState.Error )
  {
	  if (GUI.Button( Rect(20,50,80,20), "Shop" ))
	  {
	    if(uiState == UIState.Shop) { uiState = UIState.Started; }
	    else
	    {
	      if( ! roar_.Shop.hasDataFromServer )
          {
            roar_.Shop.fetch(
              function(d:Roar.CallbackInfo)
              {
                Debug.Log("Called Shop.fetch .. got :" + Roar.Json.ObjectToJSON(d)); 
              } );
          }
	      uiState = UIState.Shop;
	    }
	  }
	  
    if (GUI.Button( Rect(20,80,80,20), "Inventory" ))
    {
      if(uiState==UIState.Inventory) { uiState = UIState.Started; }
      else
      {
        if( ! roar_.Inventory.hasDataFromServer )
        {
          roar_.Inventory.fetch(
           function(d:Roar.CallbackInfo)
           {
             Debug.Log("Called Inventory.fetch .. got :" + Roar.Json.ObjectToJSON(d)); 
           } );
        }
        uiState = UIState.Inventory;
      }
    }

    if (GUI.Button( Rect(20, 110,80, 20), "Actions" ))
    {
      if(uiState==UIState.Events) { uiState = UIState.Started; }
      else
      {
        if( ! roar_.Actions.hasDataFromServer )
        {
          roar_.Actions.fetch(
           function(d:Roar.CallbackInfo)
           {
             Debug.Log("Called Events.fetch .. got :"+Roar.Json.ObjectToJSON(d)); 
           } );
        }
        uiState = UIState.Events;
      }
    }
  }


  // -- Credits button
  if (GUI.Button( Rect(w-90,10,80,20), "Get Credits" )) 
  {
    Application.OpenURL('http://google.com/');
  }


  // -- Status bar
  // GUI.Box( Rect(120,10, w-120-100,30), "");
  var gamecoins:String = '-';
  var credits:String = '-';
  if (gotProps) 
  { 
    gamecoins = roar_.Properties.getValue('gamecoins') as String;
    credits   = roar_.Properties.getValue('premium_web') as String;
  }
  GUI.Label( Rect(130,10,40,20), "Coins:");
  GUI.Label( Rect(170,10,40,20), gamecoins);
  GUI.Label( Rect(220,10,50,20), "Credits:");
  GUI.Label( Rect(270,10,40,20), credits);

  



  // -- The Listviewer
  var listX = 40;
  var listY = 120;
  var listH = h-listX-10;
  var listW = w-listY-10;
  GUI.Box( Rect(listY,listX,listW,listH), '');

  GUILayout.BeginArea( Rect(listY,listX,listW,listH) );
  scroll = GUILayout.BeginScrollView( scroll,false, true );
    
  if(uiState==UIState.Shop)
  {
    ShopUI();
  }
  else if(uiState==UIState.Inventory)
  {
    InventoryUI();
  }
  else if(uiState==UIState.Events)
  {
    ActionsUI();
  }
  else if(uiState==UIState.LoadingPlayer)
  {
    GUILayout.Label("Please wait while I load.");
  }
  else if(uiState==UIState.Error)
  {
    GUILayout.Label("Error: "+uiErrorMessage );
  }
  
  // End the scrollview we began above.
  GUILayout.EndScrollView ();

  GUILayout.EndArea();
}
