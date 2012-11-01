using UnityEngine;
using System.Collections;

public enum AlignmentHorizontal
{
	Left = 0,
	Center = 1,
	Right = 2
}

public enum AlignmentVertical
{
	Top = 0,
	Center = 1,
	Bottom = 2
}

public abstract class RoarModule : MonoBehaviour
{
	public enum BackgroundType { None, SolidColor, Image, ExtentedImage };
	
	public GUISkin customGUISkin;
	public RoarModuleController uiController;
	public RoarModule parent;
	public int depth;
	public bool constrainToBounds = false;
	public Rect bounds;
	public AlignmentHorizontal horizontalContentAlignment = AlignmentHorizontal.Left;
	public AlignmentVertical verticalContentAlignment = AlignmentVertical.Top;
	public float horizontalContentOffset;
	public float verticalContentOffset;
	
	// background configuration
	public BackgroundType backgroundType = BackgroundType.SolidColor;
	public Color backgroundColor = Color.white;
	public Texture backgroundImage;
	public string backgroundSolidColorStyle = "RootBackground";
	public string backgroundImageStyle = "RoundedBackground";
	public float extendedBackgroundWidth;
	public float extendedBackgroundHeight;
	
	protected GUISkin skin;
	protected DefaultRoar roar;
	
	private Rect imageRect;
	
	protected virtual void Awake()
	{
		roar = DefaultRoar.Instance;		
		if (customGUISkin == null)
			skin = roar.defaultGUISkin;
		else
			skin = customGUISkin;
		useGUILayout = false;
		enabled = false;
	}
	
	protected virtual void Start()
	{
		if (backgroundType == BackgroundType.Image && backgroundImage != null)
		{
			imageRect = new Rect((bounds.width-backgroundImage.width)/2,(bounds.height-backgroundImage.height)/2,backgroundImage.width,backgroundImage.height);
		}
		else if (backgroundType == BackgroundType.ExtentedImage)
		{
			if (constrainToBounds)
				imageRect = new Rect((bounds.width-extendedBackgroundWidth)/2,(bounds.height-extendedBackgroundHeight)/2,extendedBackgroundWidth,extendedBackgroundHeight);		
			else if (parent != null)
				imageRect = new Rect((parent.bounds.width-extendedBackgroundWidth)/2,(parent.bounds.height-extendedBackgroundHeight)/2,extendedBackgroundWidth,extendedBackgroundHeight);		
		}
	}
	
	public void Enable(bool enable)
	{
		enabled = enable; // TODO: this is temporary; need to support Tweening functionality at some point
	}
	/*
	IEnumerator Enable(bool enable)
	{
	}
	*/
	
	protected virtual void OnGUI()
	{
		Color c;
		
		GUI.depth = depth;
		GUI.skin = skin;
		if (parent != null && parent.constrainToBounds)
			GUI.BeginGroup(parent.bounds);
		if (constrainToBounds)
			GUI.BeginGroup(bounds);
		switch (backgroundType)
		{
		case BackgroundType.SolidColor:
			c = GUI.color;
			GUI.color = backgroundColor;
			GUI.Label(bounds, string.Empty, backgroundSolidColorStyle);
			GUI.color = c;
			break;
		case BackgroundType.Image:
			if (backgroundImage != null)
				GUI.Label(imageRect, backgroundImage);
			break;
		case BackgroundType.ExtentedImage:
			c = GUI.color;
			GUI.color = backgroundColor;
			GUI.Label(imageRect, string.Empty, backgroundImageStyle);
			GUI.color = c;
			break;
		}		
		DrawGUI();
		if (constrainToBounds)
			GUI.EndGroup();
		if (parent != null && parent.constrainToBounds)
			GUI.EndGroup();
	}
	
	protected abstract void DrawGUI();
	
	#region Utility
	public virtual void ResetToDefaultConfiguration()
	{
		horizontalContentAlignment = AlignmentHorizontal.Left;
		verticalContentAlignment = AlignmentVertical.Top;
		horizontalContentOffset = 0;
		verticalContentOffset = 0;
		depth = 1;
		constrainToBounds = false;
		backgroundType = BackgroundType.SolidColor;
		backgroundColor = Color.white;
		backgroundImage = null;
		backgroundImageStyle = "RoundedBackground";
		extendedBackgroundWidth = 0;
		extendedBackgroundHeight = 0;
	}
	#endregion
}
