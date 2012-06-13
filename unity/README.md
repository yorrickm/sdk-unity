# Roar Unity Client Library

## Get Started

The best and easiest way to get started is to:

- Download the prebuilt [Unity package](https://github.com/downloads/roarengine/sdks/RoarIO.unityPackage)
- Import the package into a new Unity project
- Drag the RoarIO object onto your main camera
- And start making calls!


## Building the roar unity package

To build the Unity SDK yourself, you will need to have node.js installed for the build process to work.
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

And finally you can build the unity package

~~~
make -f package.Makefile
~~~
