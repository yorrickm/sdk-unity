
all: package

package : clean
	mkdir -p dist/package
	# ---- create the empty unity project
	/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -createProject dist/package/roario -logFile dist/package/1-create-project.log 
	#
	# ---- copy the roar files to the project
	cp -r plugins/ dist/package/roario/Assets/Plugins/
	zip -r dist/package/roario/Assets/Plugins/roario-documentation.zip docs/html/*
	#
	# ---- export the Assets from the project
	/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -exportPackage Assets/Plugins RoarIO.unityPackage -logFile dist/package/2-export-assets.log
	# put the unity package in an easy to find location
	cp dist/package/roario/RoarIO.unityPackage dist/
	#
	# ---- Package complete!
	# ---- Unity package available at dist/RoarIO.unityPackage
	#
clean:
	# ---- clear out any existing package generation files
	rm -rf dist/package/*
