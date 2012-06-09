
docs :
	rm -rf docs/xml/*
	rm -rf docs/html/*
	/Applications/Doxygen.app/Contents/Resources/doxygen api_docs.doxygen
	#python docs_to_js.py
	#cp data.js ../codeconsole/data.js

linuxdocs :
	rm -rf docs/xml/*
	rm -rf docs/html/*
	/usr/bin/doxygen api_docs.doxygen
	#python docs_to_js.py

.PHONY : docs
