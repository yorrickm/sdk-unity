
-------------------------------------------------------
How to setup the Roar Unity SDK AngryBots demo project in Unity
-------------------------------------------------------

PART 1: Import of project Assets

To try out the AngryBots roar unity sdk demo application 3 artifacts are required.

1. The AngryBots Unity package from the Unity Asset store
http://u3d.as/content/unity-technologies/angry-bots/2aL

2. The latest augmented AngryBots roar unity files from
https://github.com/downloads/roarengine/sdks/roarunitysdk-angrybots.zip

3. The latest stable roar unity sdk package from
https://github.com/downloads/roarengine/sdks/Roar.unityPackage

The process is simple, first you import the AngryBots unity package into a new Unity project,
at this stage you should run the demo via the play button to ensure the vanilla demo is
functioning as expected. You can either download the AngryBots demo from the Unity Asset Store
or in recent versions of Unity you will find that AngryBots comes as part of the install
and can be imported during the creation of a new Unity project.

The next step is to import the AngryBots Roar Unity sdk demo files from this project into your new Unity project.
The Assets folder contains source code to augment the original AngryBots tech demo.
NOTE: The contents of the Assets folder should be overlaid on top of existing folders where necessary,
some zip applications will simply replace folders that pre-exist rather than splicing the 
contents of the pre-existing folder with the contents of a new folder with the same name.
 
For example, on a mac, you would use the ditto command to ensure AngryBots source files are not nuked when you extract the zip:

~~~
ditto -V Assets/ ~/MyAngryBotsUnityProject/Assets/
~~~

Finally you will need to import the Roar Unity sdk package via the menu item at
Assets->Import Package->Custom Package...

PART 2: 

After you have imported the 3 artifacts into your project, you will need to
setup two empty game objects to attach a series of scripts to.

First open the AngryBots scene by double-clicking it from within the Project panel (it's the
one with the black and white Unity icon).

Next create an empty Game Object and name it Roar via the menu item at
Game Object->Create Empty

From the Project panel, drag the Roar.cs file within the Plugins/Roar/ folder onto the Roar game object.

Next create an empty game object and name it RoarDemo via the menu item at
Game Object->Create Empty

Then drag and drop the following scripts from the Project panel onto the RoarDemo game object.

Demo/RoarDemo.js
Demo/EquipmentManager.js
Demo/ConfirmBox.js
Standard assets/GameScore.js

Select the RoarDemo game object in the Hierarchy panel

The Inspector panel will show the scripts that are attached to the RoarDemo game object,
note the RoarDemo script has a number of public variables exposed, including Pause Icon and Demo Skin. 
By clicking the Asset selector found at the right most region of each variable row (it looks like a tiny
circle), set the Pause Icon to the Pause texture and set the Demo Skin to the DemoGUISkin.

You are now ready to run the demo :)

