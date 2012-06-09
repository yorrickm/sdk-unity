
ifneq ($(wildcard /Applications/Doxygen.app/Contents/Resources/doxygen),)
	DOXYGEN = /Applications/Doxygen.app/Contents/Resources/doxygen
else ifneq($(wildcard /usr/bin/doxygen),)
	DOXYGEN = /usr/bin/doxygen
else
	DOXYGEN = doxygen
endif

docs :
	rm -rf docs/xml/*
	rm -rf docs/html/*
	$(DOXYGEN) api_docs.doxygen

.PHONY : docs
