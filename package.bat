@echo off
@echo.
IF EXIST dist\package rd /s/q dist
mkdir dist
mkdir dist\package
@echo ------------------------------------
@echo creating the empty unity project
@echo ------------------------------------
Unity -quit -batchmode -createProject .\dist\package\roar -logFile .\dist\package\1-create-project.log
@echo copy the roar files to the project
mkdir dist\package\roar\Assets
robocopy src\Assets dist\package\roar\Assets *.* /S /E
@echo ------------------------------------
@echo export the Assets from the project
@echo ------------------------------------
Unity  -quit -batchmode -exportPackage Assets Roar.unityPackage -logFile dist\package\2-export-assets.log
@echo ------------------------------------
@echo put the unity package in an easy to find location
@echo ------------------------------------
robocopy dist\package\roar dist Roar.unityPackage
@echo ------------------------------------
@echo Package complete!
@echo ------------------------------------
@echo Unity package available at dist\Roar.unityPackage
@echo ------------------------------------
