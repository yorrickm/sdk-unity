# Roar Unity SDK

## Using the roar sdk unity package

[A video tutorial on Setting up the RoarEngine Unity SDK package is available on our YouTube channel: http://www.youtube.com/watch?v=JBuFvBeIzuI](http://www.youtube.com/watch?v=JBuFvBeIzuI)

1. The latest Roar.unityPackage can be downloaded from 
https://github.com/roarengine/sdks/downloads 
and imported into a newly created or existing unity project.

2. Run the Unity Editor and open the project that you wish to use Roar with or create a new one.

3. Import the Roar Unity sdk package via the menu item at
Assets->Import Package->Custom Package...

4. Ensure that all files are selected for import and click 'Import'.
This will add a Plugins/Roar folder to your project.

5. Next create a Roar game object via the menu item at
Game Object->Create Other->Roar->Scene Object

6. To begin using the Roar SDK, set the game key. This is the same
name that you used when you created the game from the Roar Admin.
e.g. http://api.roar.io/yourgamekey

At this point you will be able to make calls to IRoar to interact with the roar server.

There are two ways to handle the results of a roar sdk function.

For each interaction with the roar sdk interface, you will need to either setup handlers 
for events that have been fired as a result of an sdk call or you can pass a callback 
function to an sdk call, either approach enables you to provide logic to handle the 
results of the call.

For example, to handle the successful login of a user via an event, you could write:

    function doLogin() {
       function doLogin(username, password) {
       roar.login(username, password, null);
    }
    
    RoarManager.loggedInEvent += onLogin;
    function onLogin()
    {
       ...
    }

or to handle the successful login of a user via a callback, you could write:

    function doLogin() {
       function doLogin(username, password) {
       roar.login(username, password, onLogin);
    }
    
    function onLogin(d:Roar.CallbackInfo)
    {
       ...
    }

Both of these approaches are fine, your approach will depend on which interface functions
have roar sdk events and which support callbacks. For functions that support both, the
choice is yours. 

In summary, these are the steps to get the Roar SDK set up in your Unity application.

1. Import the roar unity sdk into your project
2. Create a Roar Game Object
3. Set the roar game name (you will need to create a game via the roar admin website)
4. Configure roar sdk event handlers and callbacks
5. Update your application to make calls to the roar server via the sdk interface IRoar

For a simple example of using the Roar Unity SDK, you can try our augmented version of the Unity AngryBots tech demo.

## Building the Roar Unity package (Mac/Linux)

You will need to have node.js installed for the build process to work.
Inparticular we use jsonlint and underscore for generating some code from templates.
You can install these by running the following command from the unity directory.

    npm install -g jsonlint
    npm install underscore

You can then build the generated source code by running 

    ./build.sh

You can then build the API documentation. This will require that you have doxygen installed.

    make -f docs.Makefile

And finally you can build the roar unity sdk package

    make -f package.Makefile

This will produce the roar unity sdk package file Roar.unityPackage, which is to be used as described in
the first section of this readme.

## Building the roar unity package (Windows)

You will need to have npm and node.js installed for the build process to work.
We use jsonlint and underscore for generating some code from templates.
The node package manager (npm) will come with the node installer: http://nodejs.org/#download

After you have installed node/npm make sure they are available in the windows command
prompt by executing the following commands:

    node -v
    npm -v

If the commands do not exist check that the binaries for node and npm are on your path.

The next step is to install jsonlint and underscore by running the following npm command from the unity directory.

    cd sdks/unity
    npm install -g jsonlint
    npm install underscore

You can then build the generated source code by running

    build.bat

You can then build the API documentation. This will require that you have doxygen installed on the command line.

    doxygen api_docs.doxygen

And finally you can build the roar unity sdk package
Before running this batch script from the command line, you will need to add the Unity editor executable to
your windows PATH variable. The addition to PATH will be either:

    ;C:\Program Files (x86)\Unity\Editor

or 

    ;C:\Program Files\Unity\Editor

depending on the location of your Unity installation.

You can then run the package script to build the unity package for the roar sdk.

    package.bat

This will produce the roar unity sdk package file Roar.unityPackage, which is to be used as described in
the first section of this readme.

## Secure Communication with RoarEngine

By default the config setting for the roar server uses the https protocol, if you're Unity client does not support
https you can change the config item at application startup to use unencrypted communication:

roar.Config.roar_api_url = "http://api.roar.io/";

However it should be noted that player usernames and passwords will be plain text when using http and this approach is discouraged.