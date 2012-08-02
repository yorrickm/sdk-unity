@echo off
@echo.
IF EXIST dist\package rd /s/q dist
mkdir dist
mkdir dist\package
@echo ------------------------------------
@echo creating the empty unity project
@echo ------------------------------------
Unity -quit -batchmode -createProject .\dist\package\roario -logFile .\dist\package\1-create-project.log 
@echo copy the roar files to the project
mkdir dist\package\roario\Assets\Plugins
robocopy plugins dist\package\roario\Assets\Plugins *.* /S /E
@echo ------------------------------------
@echo export the Assets from the project
@echo ------------------------------------
Unity  -quit -batchmode -exportPackage Assets\Plugins RoarIO.unityPackage -logFile dist\package\2-export-assets.log
@echo ------------------------------------
@echo put the unity package in an easy to find location
@echo ------------------------------------
robocopy dist\package\roario dist RoarIO.unityPackage
@echo ------------------------------------
@echo Package complete!
@echo ------------------------------------
@echo Unity package available at dist\RoarIO.unityPackage
@echo ------------------------------------