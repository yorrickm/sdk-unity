using UnityEngine;
using System.Collections;
using System;

/**
 * This class is a fake! you should replace it with a real version that does whatyou need.
 * It's modeled closely on the prime31 Social Networking plugins interface.
 */

public class FacebookManager : MonoBehaviour
{
	public static void OnLogin() { loginSucceededEvent(); }
    public static void OnLogout() { loggedOutEvent(); }
    public static void OnOAuthTokenReady() { oauthTokenReady(); }

    public static event Action loginSucceededEvent;
    public static event Action<string> loginFailedEvent;
	public static event Action loggedOutEvent;
    public static event Action oauthTokenReady;
}

