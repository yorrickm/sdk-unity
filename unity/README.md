# Roar Unity SDK

## Using the roar sdk unity package

The RoarIO.unityPackage and can be imported into a newly created or existing unity project.

Run the Unity Editor and open the project that you wish to use roar with or create a new one.

Import the RoarIO Unity client package via the menu item at
Assets->Import Package->Custom Package...

Ensure that all files are selected for import and click 'Import'.
This will add a Plugins/RoarIO folder to your project.

After you have imported the roar sdk package into your unity project, you will need to
setup an empty game object to attach the primary Roar script to.

First ensure your project scene is open by double-clicking it from within the Project panel (it's the
one with the black and white Unity icon) - create the scene if you don't already have one.

Next create an empty game object and name it Roar via the menu item at
Game Object->Create Empty

From the Project panel, drag the RoarIO.cs file within the Plugins/RoarIO/ folder onto the Roar game object.

Select the Roar game object in the Hierarchy panel

The Inspector panel will show the scripts that are attached to the Roar game object.
Ensure that RoarIO.cs is attached to the Roar object.

Using the roar sdk from your application is done through the interface IRoarIO.
You can setup access to this interface during initialization of your Unity application:

~~~
var roar_:IRoarIO;
function Awake()
{
  roar_ = GetComponent(RoarIO) as IRoarIO;
}
~~~

Additionally the use of a lieghtweight or system based XML parser must be selected,
for the leightweight XML parser use:

~~~
IXMLNodeFactory.instance = new XMLNodeFactory();
~~~

and for the system XML parser use:

~~~
IXMLNodeFactory.instance = new SystemXMLNodeFactory();
~~~

We recommend using the lieghtweight implementation of the IXMLNodeFactory, however
if you think that there may be something amiss then you can switch to the system
implementation to compare behaviour.

To begin using the roar sdk, at a minimum the game name must be set. This is the same
name that you have used for the game in the roar admin website.

~~~
function Start()
{
  roar_.Config.game = "MyGameName";
}
~~~

At this point you will be able to make calls to IRoarIO to interact with the roar server.

There are two ways to handle the results of a roar sdk function.

For each interaction with the roar sdk interface, you will need to either setup handlers 
for events that have been fired as a result of an sdk call or you can pass a callback 
function to an sdk call, either approach enables you to provide logic to handle the 
results of the call.

For example, to handle the successful login of a user via an event, you could write:

~~~
function doLogin() {
   function doLogin(username, password) {
   roar.login(username, password, null);
}

RoarIOManager.loggedInEvent += onLogin;
function onLogin()
{
   ...
}
~~~

or to handle the successful login of a user via a callback, you could write:

~~~
function doLogin() {
   function doLogin(username, password) {
   roar.login(username, password, onLogin);
}

function onLogin(d:Roar.CallbackInfo)
{
   ...
}
~~~

Both of these approaches are fine, your approach will depend on which interface functions
have roar sdk events and which support callbacks. For functions that support both, the
choice is yours. 

In summary, these are the steps to get the roar sdk setup in your Unity application.

1. Import the roar unity sdk into your project
2. Create a Roar empty Game Object
3. Attach the RoarIO.cs file to the Game Object
4. For frequent use, make the IRoarIO interface a member of your game class(es) and assign it via GetComponent()
5. Set the IXMLNodeFactory.instance
6. Set the roar game name (you will need to create a game via the roar admin website)
7. Configure roar sdk event handlers and callbacks
8. Update your application to make calls to the roar server via the sdk interface IRoarIO

For a simple example of using the roar unity sdk, you can try our augmented version of the Unity AngryBots tech demo.

## Building the roar unity package (Mac/Linux)

You will need to have node.js installed for the build process to work.
Inparticular we use jsonlint and underscore for generating some code from templates.
You can install these by running the following command from the unity directory.

~~~
npm install -g jsonlint
npm install underscore
~~~

You can then build the generated source code by running 

~~~
./build.sh
~~~

You can then build the API documentation. This will require that you have doxygen installed.

~~~
make -f docs.Makefile
~~~

And finally you can build the roar unity sdk package

~~~
make -f package.Makefile
~~~

This will produce the roar unity sdk package file RoarIO.unityPackage, which is to be used as described in
the first section of this readme.

## Building the roar unity package (Windows)

You will need to have npm and node.js installed for the build process to work.
We use jsonlint and underscore for generating some code from templates.
The node package manager (npm) will come with the node installer: http://nodejs.org/#download

After you have installed node/npm make sure they are available in the windows command
prompt by executing the following commands:

~~~
node -v
npm -v
~~~

If the commands do not exist check that the binaries for node and npm are on your path.

The next step is to install jsonlint and underscore by running the following npm command from the unity directory.

~~~
cd sdks/unity
npm install -g jsonlint
npm install underscore
~~~

You can then build the generated source code by running

~~~
build.bat
~~~

You can then build the API documentation. This will require that you have doxygen installed on the command line.

~~~
doxygen api_docs.doxygen
~~~

And finally you can build the roar unity sdk package
Before running this batch script from the command line, you will need to add the Unity editor executable to
your windows PATH variable. The addition to PATH will be either:

~~~
;C:\Program Files (x86)\Unity\Editor
~~~
or 
~~~
;C:\Program Files\Unity\Editor
~~~

depending on the location of your Unity installation.

You can then run the package script to build the unity package for the roar sdk.

~~~
package.bat
~~~

This will produce the roar unity sdk package file RoarIO.unityPackage, which is to be used as described in
the first section of this readme.

## Secure Communication with RoarEngine

By default the config setting for the roar server uses the https protocol, if you're Unity client does not support
https you can change the config item at application startup to use unencrypted communication:

roar.Config.roar_api_url = "http://api.roar.io/";

However it should be noted that player usernames and passwords will be plain text when using http and this approach is discouraged.
