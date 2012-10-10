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

	private RoarModulePanel currentModule;
	
	protected override void Awake()
	{
		base.Awake();
		enabled = !disableOnAwake;
		constrainToBounds = true;
		
		// set up background rectangle
		if (fullScreenBackground)
		{
			bounds = new Rect(0, 0, Screen.width, Screen.height);
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
