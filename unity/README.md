# Roar Unity Client Library

## Building the roar unity package

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

And finally you can build the unity package

~~~
make -f package.Makefile
~~~


## Using the roar unity package

TODO: Add some docs here
