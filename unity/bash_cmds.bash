
UNITY_ASSET_DIR=/Users/michaelanderson/UnityRoarIO/Assets

u_pull()
{
    cp ${UNITY_ASSET_DIR}/DemoApp.js . 
    cp -r ${UNITY_ASSET_DIR}/plugins/* plugins
}

u_push()
{
	cp DemoApp.js ${UNITY_ASSET_DIR}/
	test -d ${UNITY_ASSET_DIR}/plugins || mkdir ${UNITY_ASSET_DIR}/plugins
	cp -r plugins/* ${UNITY_ASSET_DIR}/plugins
}

u_check()
{
	diff -u ${UNITY_ASSET_DIR}/DemoApp.js DemoApp.js
	diff -r plugins ${UNITY_ASSET_DIR}/plugins
}
