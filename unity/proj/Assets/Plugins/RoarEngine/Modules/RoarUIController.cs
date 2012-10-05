using UnityEngine;
using System.Collections;

public enum RoarUIPanel
{ 
	Off		= -1, 
	Login	= 0
}

public class RoarUIController : RoarUI
{
	public bool disableOnAwake = true;
	public bool fullScreenBackground = true;
	public RoarUI[] uiModules;

	private RoarUIPanel currentUI;
	
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
	{
	}
	
	public RoarUIPanel UIPanel
	{
		get { return currentUI; }
		
		set
		{ 
			int moduleIndex = (int)value;
			if (moduleIndex != -1 && (moduleIndex >= uiModules.Length || uiModules[moduleIndex] == null))
				throw new System.Exception("Requested UI is not attached to RoarUIController.");
			
			// disable the current panel
			if (currentUI != RoarUIPanel.Off)
				uiModules[(int)currentUI].Enable(false);
			currentUI = value;
			
			// disable/enable the root
			Enable(value != RoarUIPanel.Off);
			
			// enable the new current
			if (currentUI != RoarUIPanel.Off)
				uiModules[(int)currentUI].Enable(true);				
		}
	}
}
