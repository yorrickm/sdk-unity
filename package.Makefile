ifneq ($(wildcard /Applications/Unity/Unity.app/Contents/MacOS/Unity),)
	UNITY = /Applications/Unity/Unity.app/Contents/MacOS/Unity
else
	UNITY = Unity
endif

all: package

package : clean
	mkdir -p dist/package
	# ---- create the empty unity project
	$(UNITY) -quit -batchmode -createProject dist/package/roar -logFile dist/package/1-create-project.log 
	#
	# ---- copy the roar files to the project
	cp -r plugins/ dist/package/roar/Assets/Plugins/
	zip -r dist/package/roar/Assets/Plugins/roar-documentation.zip docs/html/*
	#
	# ---- export the Assets from the project
	$(UNITY) -quit -batchmode -exportPackage Assets/Plugins Roar.unityPackage -logFile dist/package/2-export-assets.log
	# put the unity package in an easy to find location
	cp dist/package/roar/Roar.unityPackage dist/
	#
	# ---- Package complete!
	# ---- Unity package available at dist/Roar.unityPackage
	#
clean:
	# ---- clear out any existing package generation files
	rm -rf dist/package/*
