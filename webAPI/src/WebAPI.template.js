#pragma strict

static var UNKNOWN_ERR:int  = 0;    // Default unspecified error (parse manually)
static var UNAUTHORIZED:int = 1;    // Auth token is no longer valid. Relogin.
static var BAD_INPUTS:int   = 2;    // Incorrect parameters passed to Roar
static var DISALLOWED:int   = 3;    // Action was not allowed (but otherwise successful)
static var FATAL_ERROR:int  = 4;    // Server died somehow (sad/bad/etc)
static var AWESOME:int      = 11;   // Turn it up.
static var OK:int           = 200;  // Everything ok - proceed

class WebAPI
{

  public var roarAuthToken:String = '';

  // Set these values to your Roar game values
  var roar_api_url:String = "http://api.roar.io/";
  
  // `gameKey` is exposed and set in Roar as a public UnityEditor variable
  public var gameKey:String = '';


  function _sendCore( apicall:String, params:Boo.Lang.Hash, cb:Function, opt:Boo.Lang.Hash)
  {
    if (gameKey == '')
    {
      if (Roar.instance.debug) Debug.Log('[roar] -- No game key set!--');
      return;
    }

    if (Roar.instance.debug) Debug.Log('[roar] -- Calling: '+apicall);

    // Encode POST parameters
    var post:WWWForm = new WWWForm() as WWWForm;
    for (var param:DictionaryEntry in params)
    {
      post.AddField( param.Key as String, param.Value as String );
    }
    // Add the auth_token to the POST
    post.AddField( 'auth_token', roarAuthToken );

    // Fire call sending event
    RoarManager.OnRoarNetworkStart();

    var xhr = WWW( roar_api_url+gameKey+"/"+apicall+"/", post);
    yield xhr;

    // if (Roar.instance.debug) Debug.Log(xhr.text);

    onServerResponse( xhr.text, apicall, cb, opt );
  }

  // Take a Boo.Lang.Hash that has been processed by ToHashDeep and remove _ attributes
  static function StripHashOfUnderscoreAttribs( parse:Boo.Lang.Hash ) : Boo.Lang.Hash
  {
    var thisLevel = {};
    // Step through the object at this level
    for (var param:DictionaryEntry in parse)
    {    
      var key:String = (param.Key as String);

      // XMLNodeLists must be parsed and added to this level recursively
      if (typeof(param.Value) == ArrayList)
      {
        var ar:ArrayList = param.Value as ArrayList;
        
        for (var i:int=0; i< ar.Count; i++)
        {
          if ( !thisLevel[key] ) thisLevel[key] = new ArrayList();
          // Only process XMLNodes (as Hashes of course)
          if (typeof(ar[i]) == Boo.Lang.Hash) 
              (thisLevel[key] as ArrayList).Add( ToHashDeep( ar[i] as Boo.Lang.Hash ) );
        }
      }
      // We're dealing with strings
      // But strip out anything starting with '_' (XML Parser crap)
      else if ( (key)[0] != '_' ) 
      {
        // Convert string booleans to native booleans
        if (param.Value == 'true') param.Value = true;
        else if (param.Value == 'false') param.Value = false;

        thisLevel[ key ] = param.Value;
      }
    }
    return thisLevel as Boo.Lang.Hash;
  }

  // Converts a Boo.Lang.Hash object with nexted XMLNodeLists (ie. output from
  // the XML Parser) to a pure, neat Boo.Lang.Hash
  static function ToHashDeep( parse:Boo.Lang.Hash ) : Boo.Lang.Hash { return ToHashDeep( parse, false ); }
  static function ToHashDeep( parse:Boo.Lang.Hash, preserve:boolean ) : Boo.Lang.Hash
  {
    var thisLevel = {};
    // Step through the object at this level
    for (var param:DictionaryEntry in parse)
    {    
      var key:String = (param.Key as String);

      // XMLNodeLists must be parsed and added to this level recursively
      if (typeof(param.Value) == XMLNodeList)
      {
        var ar:XMLNodeList = param.Value as XMLNodeList;
        
        for (var i:int=0; i< ar.length; i++)
        {
          if ( !thisLevel[key] ) thisLevel[key] = new ArrayList();
          // Only process XMLNodes (as Hashes of course)
          if (typeof(ar[i]) == XMLNode) 
              (thisLevel[key] as ArrayList).Add( ToHashDeep( ar[i] as Boo.Lang.Hash, preserve ) );
        }
      }
      // We're dealing with strings
      // But strip out anything starting with '_' (XML Parser crap)
      else if ( (key)[0] != '_' || preserve ) 
      {
        // Convert string booleans to native booleans
        if (param.Value == 'true') param.Value = true;
        else if (param.Value == 'false') param.Value = false;

        thisLevel[ key ] = param.Value;
      }
    }
    return thisLevel as Boo.Lang.Hash;
  }


  function onServerResponse( raw:String, apicall:String, cb:Function, opt:Boo.Lang.Hash )
  {
    var uc = apicall.Split("/"[0]);
    var controller = uc[0];
    var action = uc[1];

    var call_id = 0; // TODO: Hook this up to History
    
    // Fire call complete event
    RoarManager.OnRoarNetworkEnd(call_id.ToString() );

    // -- Parse the Roar response
    // Unexpected server response 
    if (raw[0] != '<')
    {
      // Error: fire the error callback
      if (cb) cb( raw, FATAL_ERROR, 'Invalid server response', call_id, opt );
      return;
    }

    // XML Parser: http://dev.grumpyferret.com/unity/
    // (with fix to handle self terminating nodes eg. <node/>)
    var parser:XMLParser = new XMLParser();
    var node:XMLNode = parser.Parse( raw );

    var callback_code:int;
    var callback_msg:String;

    // XMLNode extends Boo.Lang.Hash
    // XMLNodeList is Array - must point to node sibling level (not parent)
    var d:XMLNode = node.GetNode( "roar>0>"+controller+">0>"+action+'>0' );
    // Hash XML keeping _name and _text values by default
    var callXmlHash = ToHashDeep( d, true );

    // Pre-process <server> block if any and attach any processed data
    var serverHash = processServerChunk( node );
    if (serverHash) callXmlHash['server'] = serverHash;

    // Status on Server returned an error. Action did not succeed.
    var status:String = node.GetValue( 'roar>0>'+controller+'>0>'+action+'>0>status' );
    if (status == 'error')
    {
      callback_code = UNKNOWN_ERR;
      callback_msg = node.GetValue( 'roar>0>'+controller+'>0>'+action+'>0>error>0>_text' );
      var server_error = node.GetValue( 'roar>0>'+controller+'>0>'+action+'>0>error>0>@type' );
      if ( server_error == '0' )
      {
        if (callback_msg==='Must be logged in') { callback_code = UNAUTHORIZED; }
        if (callback_msg==='Invalid auth_token') { callback_code = UNAUTHORIZED; }
        if (callback_msg==='Must specify auth_token') { callback_code = BAD_INPUTS; }
        if (callback_msg==='Must specify name and hash') { callback_code = BAD_INPUTS; }
        if (callback_msg==="Invalid name or password") { callback_code = DISALLOWED; }
        if (callback_msg==="Player already exists") { callback_code = DISALLOWED; }
      }

      // Error: fire the callback
      // NOTE: The Unity version ASSUMES callback = errorCallback
      if (cb) cb( callXmlHash, callback_code, callback_msg, call_id, opt );      
    }

    // No error - pre-process the result
    else
    {
      var auth_token:XMLNode = node.GetNode( "roar>0>"+controller+">0>"+action+'>0>auth_token>0' );
      if (auth_token) roarAuthToken = auth_token['_text'] as String;
      
      callback_code = OK;
      if (cb) cb( callXmlHash, callback_code, callback_msg, call_id, opt );
    }

    RoarManager.OnCallComplete( RoarManager.CallInfo(node, callback_code, callback_msg, call_id.ToString() ) );
  }


  // Checks for a <server> node and broadcasts events
  // for any matching chunks
  private function processServerChunk( node:XMLNode ) : Boo.Lang.Hash
  {
    var retData:Boo.Lang.Hash = null;

    var server:XMLNode = node.GetNode( 'roar>0>server>0' );
    if (! server) return retData;

    // Dispatch the entire <server> object
    RoarManager.OnRoarServerAll(server);
    for (var chunk:DictionaryEntry in server as Boo.Lang.Hash)
    {
      var key:String = chunk.Key as String;

      // Setup initial event node data (note the TRUE preserve parameter)
      var chunkData = ToHashDeep( node.GetNode( 'roar>0>server>0>'+key+'>0'), true);

      // Reprocess the <task_complete> server chunk
      if ( key == 'task_complete' )
      {
        var replace = parseTaskComplete( chunkData );
        chunkData = replace;

        // Attach reformatted object to return data
        retData = replace;
      }
      
      // Dispatch each individual <server> node
      // - nodes starting with '_' are from XMLParser conversion
      //   and not needed, so just ignore
      if ( (key as String)[0] != '_')
      {
        switch(key)
        {
          case "update":
            RoarManager.OnRoarServerUpdate(chunkData);
            break;
          case "item_use":
            RoarManager.OnRoarServerItemUse(chunkData);
            break;
          case "item_lose":
            RoarManager.OnRoarServerItemLose(chunkData);
            break;
          case "inventory_changed":
            RoarManager.OnRoarServerInventoryChanged(chunkData);
            break;
          default:
            Debug.Log("Server event "+key+" not yet implemented");
        }
      }
      
    }

    return retData;
  }

  // -- Task complete data is send back in the <server> block
  // NOTE: task_complete is remapped to 'standard' format as because
  // server sends back 'nonstandard' <node>value</node> rather than
  // as properties/attributes.
  private function parseTaskComplete( taskChunk:Boo.Lang.Hash ) : Boo.Lang.Hash
  {
    var replace = {};
    for (var tasknode:DictionaryEntry in taskChunk)
    {
      var ik:String = tasknode.Key as String;

      // Cut out the `location` and `mastery` fields
      if (ik == 'location' ) continue;
      if (ik == 'mastery') continue;

      if (typeof(tasknode.Value) != ArrayList)
      {
        if (ik[0] != '_') replace[ik] = tasknode.Value;
        continue;
      }

      var iar:ArrayList = tasknode.Value as ArrayList;
      var inode = iar[0] as Boo.Lang.Hash;

      // Build a flat tag array
      if (ik == 'tags')
      {
        var copyTags = new ArrayList();
        var tags = inode['tag'] as ArrayList;
        if(tags!=null)
        {
          for (var itag:int=0; itag<tags.Count; itag++)
          {
            copyTags.Add( (tags[itag] as Boo.Lang.Hash)['value'] );
          }
        }
        replace[ik] = copyTags;
      }
      // Process all other nodes as _text except `mods` and `costs`
      else if (ik != 'modifiers' && ik != 'costs')
      {
        replace[ ik ] = inode['_text'];
      }
      // ..which are processed as Hashes
      else replace[ik] = ToHashDeep( inode as Boo.Lang.Hash, false );

    }

    return replace;
  }



  // --- Roar Web API implementation:
<%
  var pad_str="            ";
  _.each( data.modules, function(m,i,l) {
    print("  public var " + pad_right(m.name,pad_str) +" = new "+capitalizeFirst(m.name)+"Actions( this );\n")
    } )
%>
  
  class APIBridge
  {
    var api:WebAPI;
    function APIBridge( caller:WebAPI ) { api = caller; }
  }

<%
  _.each( data.modules, function(m,i,l) {
    var class_name = capitalizeFirst(m.name)+"Actions"
%>
  class <%= class_name %> extends APIBridge
  {
    function <%= class_name %>( caller:WebAPI ) { super(caller); }

<% _.each( m.functions, function(f,j,ll) {
     url = f.url ? f.url : (m.name+"/"+f.name);
     obj = f.obj ? f.obj : "obj";
     print("    function "+fix_reserved_word(f.name)+"(obj:Boo.Lang.Hash, cb:Function, opt:Boo.Lang.Hash)\n");
     print("    {\n");
     print("      Roar.instance.StartCoroutine( api._sendCore('"+url+"', "+obj+", cb, opt ));\n");
     print("    }\n\n");
} ) %>  }
<% } ) %>

}

