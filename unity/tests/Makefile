all: test

ROAR_SRC:=$(shell find ../plugins/Roar -name "*.cs")
XMLPARSER_SRC:=$(shell find ../plugins/XMLParser -name "*.cs")

clean:
	find . -name "*.dll" -delete
	cp ~/Downloads/nmock2-2.0.0.44-net-2.0/bin/NMock2.dll .

Roar.dll : $(ROAR_SRC) $(XMLPARSER_SRC) stubs/UnityEngine.cs 
	gmcs -debug -t:library -out:Roar.dll $(ROAR_SRC) $(XMLPARSER_SRC) stubs/UnityEngine.cs  

Tests.dll : Roar.dll XmlTests.cs DataConversionTests.cs WebAPITests.cs
	gmcs -out:Tests.dll -debug -target:library mocks/* ComponentTests.cs ActionsTests.cs InventoryTests.cs PropertiesTests.cs ShopTests.cs XmlTests.cs DataConversionTests.cs WebAPITests.cs -r:NUnit.Framework.dll -lib:. -r:Roar.dll -r:NMock2.dll

test : Tests.dll
	nunit-console Tests.dll
