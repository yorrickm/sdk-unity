using UnityEngine;
using System.Collections;

public class RoarShopTest : MonoBehaviour {

	static void onShopFetch( Roar.CallbackInfo info )
	{
		Debug.Log( "onShopFetch Called" );
		Debug.Log(info);
		Debug.Log(info.code);
		Debug.Log(info.msg);
		Debug.Log(info.d);	
	}
	
	static void onLogin( Roar.CallbackInfo info )
	{
		Debug.Log( "onLogin Called" );
		Debug.Log(info);
		Debug.Log(info.code);
		Debug.Log(info.msg);
		Debug.Log(info.d);
		IRoar roar = DefaultRoar.Instance;
		roar.Shop.Fetch( onShopFetch );
	}
	
	// Use this for initialization
	void Start () {
		IRoar roar = DefaultRoar.Instance;
		roar.Login("mike","mike", onLogin);
	}
	
	void OnMouseDown() {
		Debug.Log ("OnMouseDown Called");
		IRoar roar = DefaultRoar.Instance;
		roar.Login("mike","mike", onLogin);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
