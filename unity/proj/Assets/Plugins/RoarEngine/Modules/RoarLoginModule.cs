using UnityEngine;
using System.Collections;

public class RoarLoginModule : RoarModule
{
	public delegate void RoarLoginModuleHandler();
	public static event RoarLoginModuleHandler OnFullyLoggedIn; // logged in & stats fetched
	
	public float statusWidth = 320;
	public float statusHeight = 48;
	public string statusNormalStyle = "StatusNormal";
	public string statusErrorStyle = "StatusError";
	
	public string textfieldStyle = "LoginLabel";	
	public float textfieldWidth = 240;
	public float textfieldHeight = 32;
	public float textFieldSpacing = 4;
	
	public float buttonWidth = 240;
	public float buttonHeight = 32;
	public float spacingAboveButtons = 16;
	public float spacingBetweenButtons = 4;
	
	public float verticalOffset = -40;
	public bool fetchPropertiesOnLogin = true;
	public bool saveUsername = true;
	public bool savePassword = false;
	
	private static string KEY_USERNAME = "roar-username";
	private static string KEY_PASSWORD = "roar-password";
	
	private string status = "Supply a username and password to log in or to register a new account.";
	private bool isError;
	private string username = string.Empty;
	private string password = string.Empty;
#if UNITY_EDITOR
	private string latchedNameOfFocusedControl = string.Empty;
#endif
	private Rect statusLabelRect;
	private Rect usernameLabelRect;
	private Rect usernameFieldRect;
	private Rect passwordLabelRect;
	private Rect passwordFieldRect;
	private Rect submitButtonRect;
	private Rect createButtonRect;
	private bool networkActionInProgress;
	
	protected override void Awake ()
	{
		base.Awake ();
		
		if (saveUsername)
		{
			username = PlayerPrefs.GetString(KEY_USERNAME, string.Empty);
		}
		if (savePassword)
		{
			password = PlayerPrefs.GetString(KEY_PASSWORD, string.Empty);
		}
	}
	
	protected override void Start()
	{
		base.Start();
		
		float totalHeight = textfieldHeight + textFieldSpacing // label
			              + textfieldHeight + textFieldSpacing // username
			              + textfieldHeight + textFieldSpacing // password
				          + spacingAboveButtons
		                  + buttonHeight + spacingBetweenButtons // log in
				          + buttonHeight // register
		;
		float yAdjust = (parent.bounds.height/2 - totalHeight/2) + verticalOffset;
		
		// initial layout, top to bottom
		statusLabelRect = new Rect((parent.bounds.width/2 - statusWidth/2), yAdjust, statusWidth, statusHeight);
		usernameLabelRect = new Rect((parent.bounds.width/2 - textfieldWidth/2), yAdjust, textfieldWidth, textfieldHeight);
		usernameLabelRect.y += statusHeight + textFieldSpacing;
		usernameFieldRect = usernameLabelRect;
		usernameFieldRect.y += textfieldHeight + textFieldSpacing;
		passwordLabelRect = usernameFieldRect;
		passwordLabelRect.y += textfieldHeight + textFieldSpacing;
		passwordFieldRect = passwordLabelRect;
		passwordFieldRect.y += textfieldHeight + textFieldSpacing;
		submitButtonRect = new Rect((parent.bounds.width/2 - buttonWidth/2), passwordFieldRect.y, buttonWidth, buttonHeight);
		submitButtonRect.y += textfieldHeight + spacingAboveButtons;
		createButtonRect = submitButtonRect;
		createButtonRect.y += buttonHeight + spacingBetweenButtons;		
	}
	
	protected override void DrawGUI()
	{		
		GUI.Label(statusLabelRect, status, (isError) ? skin.FindStyle(statusErrorStyle) : skin.FindStyle(statusNormalStyle));
		
		GUI.Label(usernameLabelRect, "Username", textfieldStyle);
#if UNITY_EDITOR
		GUI.SetNextControlName("u");
#endif
		username = GUI.TextField(usernameFieldRect, username);
		
		GUI.Label(passwordLabelRect, "Password", textfieldStyle);
#if UNITY_EDITOR
		GUI.SetNextControlName("p");
#endif
		password = GUI.PasswordField(passwordFieldRect, password, '*');

#if UNITY_EDITOR
		string nameOfFocusedControl = GUI.GetNameOfFocusedControl();
		if (nameOfFocusedControl != string.Empty)
			latchedNameOfFocusedControl = nameOfFocusedControl;
		
		Event evt = Event.current;
		if (latchedNameOfFocusedControl == "u")
		{
			if (evt.isKey && evt.type == EventType.KeyDown)
			{
				if (evt.keyCode == KeyCode.Backspace)
				{
					if (username.Length > 0)
						username = username.Substring(0, username.Length-1);
				}
				else if (evt.character != '\0')
				{
					username += evt.character;
				}
				evt.Use();
			}
		}
		else if (latchedNameOfFocusedControl == "p")
		{
			if (evt.isKey && evt.type == EventType.KeyDown)
			{
				if (evt.keyCode == KeyCode.Backspace)
				{
					if (password.Length > 0)
						password = password.Substring(0, password.Length-1);
				}
				else if (evt.character != '\0')
				{
					password += evt.character;
				}
				evt.Use();
			}
		}
#endif
		GUI.enabled = username.Length > 0 && password.Length > 0 && !networkActionInProgress;
		
		if (GUI.Button(submitButtonRect, "Log In"))
		{
#if UNITY_EDITOR
			latchedNameOfFocusedControl = string.Empty;
#endif
			status = "Logging in...";
			networkActionInProgress = true;			
			if (saveUsername)
			{
				PlayerPrefs.SetString(KEY_USERNAME, username);
			}
			if (savePassword)
			{
				PlayerPrefs.SetString(KEY_PASSWORD, password);
			}
			if (Debug.isDebugBuild)
				Debug.Log(string.Format("[Debug] Logging in as [{0}] with password [{1}].", username, password));
			roar.Login(username, password, OnRoarLoginComplete);
		}
		if (GUI.Button(createButtonRect, "Create Account"))
		{
#if UNITY_EDITOR
			latchedNameOfFocusedControl = string.Empty;
#endif
			status = "Creating new player account...";
			networkActionInProgress = true;
			roar.Create(username, password, OnRoarAccountCreateComplete);
		}
		
		GUI.enabled = true;
	}
	
	void OnRoarLoginComplete(Roar.CallbackInfo info)
	{
		if (Debug.isDebugBuild)
			Debug.Log(string.Format("OnRoarLoginComplete ({0}): {1}", info.code, info.msg));
		switch (info.code)
		{
		case 200: // (success)
			isError = false;
			
			// fetch the player's properties after successful login
			if (fetchPropertiesOnLogin)
			{
				DefaultRoar.Instance.Properties.Fetch(OnRoarPropertiesFetched);
			}
			else
			{
				uiController.CurrentModulePanel = RoarModulePanel.Off;			
				networkActionInProgress = false;
			}
			
			break;
		case 3: // Invalid name or password
		default:
			isError = true;
			status = info.msg;
			networkActionInProgress = false;
			break;
		}
	}

	void OnRoarAccountCreateComplete(Roar.CallbackInfo info)
	{
		if (Debug.isDebugBuild)
			Debug.Log(string.Format("OnRoarAccountCreateComplete ({0}): {1}", info.code, info.msg));
		switch (info.code)
		{
		case 200: // (success)
			status = "Account successfully created. You can now log in.";
			break;
		case 3:
		default:
			isError = true;
			status = info.msg;
			break;
		}
		networkActionInProgress = false;
	}
	
	void OnRoarPropertiesFetched(Roar.CallbackInfo info)
	{
		networkActionInProgress = false;
		uiController.CurrentModulePanel = RoarModulePanel.Off;
		if (OnFullyLoggedIn != null) OnFullyLoggedIn();
	}

	#region Utility
	public override void ResetToDefaultConfiguration()
	{
		base.ResetToDefaultConfiguration();
		backgroundType = RoarModule.BackgroundType.ExtentedImage;
		backgroundColor = new Color32(199,199,199,192);
		extendedBackgroundWidth = 360;
		extendedBackgroundHeight = 324;

		statusWidth = 320;
		statusHeight = 48;
		statusNormalStyle = "StatusNormal";
		statusErrorStyle = "StatusError";
		
		textfieldWidth = 240;
		textfieldHeight = 32;
		textFieldSpacing = 4;
		
		buttonWidth = 240;
		buttonHeight = 32;
		spacingAboveButtons = 16;
		spacingBetweenButtons = 4;
		
		verticalOffset = -40;
	}
	#endregion
}
