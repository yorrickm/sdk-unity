var _ = require("../html5/libs/roarjs/external/underscore/underscore.js")

fs = require('fs')

function pad_left( v, str )
{
	if (v.length < str.length)
	{
		return  String(str+v).slice(-str.length)
	}
	return v
}

function pad_right( v, str )
{
	if (v.length < str.length)
	{
		return  String(v+str).slice(0,str.length)
	}
	return v
}

function capitalizeFirst( s )
{
  return s.charAt(0).toUpperCase() + s.slice(1);
}

function fix_reserved_word( w )
{
  if( w==="set" ) return "_set";
  return w
}

function augment_template( t )
{
  t.fix_reserved_word = fix_reserved_word
  t.pad_left = pad_left
  t.pad_right = pad_right
  t.capitalizeFirst = capitalizeFirst
}

function build_web_api_cs()
{
  api_functions = require("../html5/libs/roarjs/api_functions.json")
  js_template = fs.readFileSync('src/WebAPI.template.cs',"utf8");
  augment_template( api_functions )

  var output = _.template(js_template, api_functions)

  fs.writeFile("plugins/RoarIO/implementation/WebAPI.cs",output);
}

function build_iweb_api_cs()
{
  api_functions = require("../html5/libs/roarjs/api_functions.json")
  js_template = fs.readFileSync('src/IWebAPI.template.cs',"utf8");
  augment_template( api_functions )

  var output = _.template(js_template, api_functions)

  fs.writeFile("plugins/RoarIO/IWebAPI.cs",output);
}

function build_event_manager()
{
  events = require("./data/events.json")
  cs_template = fs.readFileSync('src/RoarIOManager.template.cs',"utf8");
  augment_template( events )

  _.each(
    events.data.components,
    function(e,i,l)
    {
      events.data.events.push(
        {
          "name":e+"Ready",
          "notes":["Fired when the data have been retrieved from the server"],
          "args":[]
        } );
      events.data.events.push(
        {
          "name":e+"Change",
          "notes":["Fired when the data changes"],
          "args":[]
        } );
    } );

  var output = _.template(cs_template, events)

  fs.writeFile("plugins/RoarIO/RoarIOManager.cs",output);
}

function build_ievent_manager()
{
  events = require("./data/events.json")
  cs_template = fs.readFileSync('src/IRoarIOManager.template.cs',"utf8");
  augment_template( events )

  var output = _.template(cs_template, events)

  fs.writeFile("plugins/RoarIO/IRoarIOManager.cs",output);
}

build_web_api_cs();
build_iweb_api_cs();
build_event_manager();
//build_ievent_manager();