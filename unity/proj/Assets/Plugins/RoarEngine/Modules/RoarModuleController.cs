using UnityEngine;
using System.Collections;

public enum RoarModulePanel
{ 
	Off				= -1, 
	Login			= 0,
	Stats			= 1,
	Leaderboards	= 2,
	Friends			= 3,
	Inventory		= 4,
	Shop			= 5,
	Gifting			= 6,
	Quests			= 7,
	Badges			= 8,
}

public class RoarModuleController : RoarModule
{
	public bool disableOnAwake = true;
	public bool fullScreenBackground = true;
	public RoarModule[] uiModules;

	private RoarModulePanel currentModule = RoarModulePanel.Off;
	private bool isLoggedIn;
	
	protected override void Awake()
	{
		base.Awake();
		enabled = !disableOnAwake;
		constrainToBounds = true;
		
		// listen for log-in event
		RoarManager.loggedInEvent -= OnRoarLogin;
		RoarManager.loggedInEvent += OnRoarLogin;
		RoarManager.loggedOutEvent -= OnRoarLogout;
		RoarManager.loggedOutEvent += OnRoarLogout;
		
		// set up background rectangle
		if (fullScreenBackground)
		{
			bounds = new Rect(0, 0, Screen.width, Screen.height);
		}
	}
	
	protected override void Start()
	{
		base.Start();
		CurrentModulePanel = RoarModulePanel.Login;
	}
	
	void OnEnable()
	{
		if (!isLoggedIn)
		{
			CurrentModulePanel = RoarModulePanel.Login;
		}
	}
	
	protected override void DrawGUI()
	{}
	
	public RoarModulePanel CurrentModulePanel
	{
		get { return currentModule; }
		
		set
		{ 
			int moduleIndex = (int)value;
			if (moduleIndex != -1 && (moduleIndex >= uiModules.Length || uiModules[moduleIndex] == null))
				throw new System.Exception("Requested module is not attached to RoarModuleController.");
			
			// disable the current panel
			if (currentModule != RoarModulePanel.Off)
				uiModules[(int)currentModule].Enable(false);
			currentModule = value;
			
			// disable/enable the root
			Enable(value != RoarModulePanel.Off);
			
			// enable the new current
			if (currentModule != RoarModulePanel.Off)
				uiModules[(int)currentModule].Enable(true);				
		}
	}
	
	void OnRoarLogin()
	{
		isLoggedIn = true;
	}

	void OnRoarLogout()
	{
		isLoggedIn = false;
	}
	
	#region Utility
	public override void ResetToDefaultConfiguration()
	{
		base.ResetToDefaultConfiguration();
		backgroundType = RoarModule.BackgroundType.SolidColor;
		backgroundColor = new Color32(188,191,198,217);
		
		disableOnAwake = true;
		fullScreenBackground = true;
	}
	#endregion
}
