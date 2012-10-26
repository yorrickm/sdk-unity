using UnityEngine;
using System.Collections;

public abstract class RoarUIWidget : MonoBehaviour
{
	public enum BoundType { Plain = 0, Boxed = 1, FixedWindow = 2, DraggableWindow = 3 };
	
	public GUISkin customGUISkin;
	
	public int depth;
	public Rect bounds;
	public Rect contentBounds;
	public Color color = Color.white;
	public BoundType boundType = BoundType.Plain;
	public string boundingStyle = "WidgetBoundingStyle";
	public string boundingTitle = string.Empty;
	public Texture boundingImage = null;
	public bool draggableWindowFullScreen = true;
	public Rect draggableWindowBounds;
	
	public AlignmentHorizontal horizontalAlignment = AlignmentHorizontal.Left;
	public AlignmentVertical verticalAlignment = AlignmentVertical.Top;
	public float horizontalOffset;
	public float verticalOffset;
	
	public bool useScrollView = false;
	public float initialContentWidth = 0;
	public float initialContentHeight = 0;
	public bool alwaysShowHorizontalScrollBar = false;
	public bool alwaysShowVerticalScrollBar = true;
	
	public bool autoEnableOnLogIn = false;
	public bool autoDisableOnLogout = true;
	
	protected GUISkin skin;
	protected DefaultRoar roar;
	
	// windowing
	private int windowId;
	private static int windowIdGenerator = 0;
	private GUIContent boundingGUIContent;
	
	// scrolling
	private Vector2 scrollPosition = Vector2.zero;
	private Rect scrollViewRect;

	private bool isLoggedIn;
	
	protected virtual void Awake()
	{
		roar = DefaultRoar.Instance;		
		if (customGUISkin == null)
			skin = roar.defaultGUISkin;
		else
			skin = customGUISkin;
		
		boundingGUIContent = new GUIContent(boundingTitle, boundingImage);
		if (draggableWindowFullScreen)
			draggableWindowBounds = new Rect(0,0,Screen.width,Screen.height);	
		scrollViewRect = bounds;
		if (initialContentWidth == 0)
			initialContentWidth = bounds.width;
		if (initialContentHeight == 0)
			initialContentHeight = bounds.height;
		scrollViewRect.width = initialContentWidth;
		scrollViewRect.height = initialContentHeight;

		// listen for log-in event
		RoarLoginModule.OnFullyLoggedIn -= OnRoarLogin;
		RoarLoginModule.OnFullyLoggedIn += OnRoarLogin;
		RoarManager.loggedOutEvent -= OnRoarLogout;
		RoarManager.loggedOutEvent += OnRoarLogout;
		
		switch (boundType)
		{
		case BoundType.FixedWindow:
		case BoundType.DraggableWindow:
			windowIdGenerator++;
			windowId = windowIdGenerator;

			// horizontal binding
			switch (horizontalAlignment)
			{
			case AlignmentHorizontal.Right:
				bounds.x = Screen.width - bounds.width;
				break;
			case AlignmentHorizontal.Center:
				bounds.x = (Screen.width - bounds.width) / 2;
				break;
			}
			bounds.x += horizontalOffset;
	
			// vertical binding
			switch (verticalAlignment)
			{
			case AlignmentVertical.Bottom:
				bounds.y = Screen.height - bounds.height;
				break;
			case AlignmentVertical.Center:
				bounds.y = (Screen.height - bounds.height) / 2;
				break;
			}
			bounds.y += verticalOffset;
			
			break;
			
		default:
			useGUILayout = false;		
			break;
		}

		enabled = false;
	}
	
	protected virtual void OnEnable()
	{
		scrollPosition = Vector2.zero;
	}
	
	void Reset()
	{
		bounds = new Rect(0,0,512,386);
	}
	
	void OnRoarLogin()
	{
		isLoggedIn = true;
		if (autoEnableOnLogIn)
		{
			enabled = true;
		}
	}

	void OnRoarLogout()
	{
		isLoggedIn = false;
		if (autoDisableOnLogout)
		{
			enabled = false;
		}
	}
	
	protected bool IsLoggedIn
	{
		get { return isLoggedIn; }
	}
	
	void OnGUI()
	{
		// rendering attributes
		GUI.skin = skin;
		GUI.depth = depth;
		GUI.color = color;
		
		if (boundType == BoundType.FixedWindow)
		{
			Rect bounds = this.bounds;
			
			// horizontal binding
			switch (horizontalAlignment)
			{
			case AlignmentHorizontal.Right:
				bounds.x = Screen.width - bounds.width;
				break;
			case AlignmentHorizontal.Center:
				bounds.x = (Screen.width - bounds.width) / 2;
				break;
			}
			bounds.x += horizontalOffset;
	
			// vertical binding
			switch (verticalAlignment)
			{
			case AlignmentVertical.Bottom:
				bounds.y = Screen.height - bounds.height;
				break;
			case AlignmentVertical.Center:
				bounds.y = (Screen.height - bounds.height) / 2;
				break;
			}
			bounds.y += verticalOffset;
			
			GUI.Window(windowId, bounds, DrawFixedWindow, boundingGUIContent, boundingStyle);
		}
		else if (boundType == BoundType.DraggableWindow)
		{
			bounds = GUI.Window(windowId, bounds, DrawDraggableWindow, boundingGUIContent, boundingStyle);
		}
		else
		{
			Rect bounds = this.bounds;
			
			// horizontal binding
			switch (horizontalAlignment)
			{
			case AlignmentHorizontal.Right:
				bounds.x = Screen.width - bounds.width;
				break;
			case AlignmentHorizontal.Center:
				bounds.x = (Screen.width - bounds.width) / 2;
				break;
			}
			bounds.x += horizontalOffset;
	
			// vertical binding
			switch (verticalAlignment)
			{
			case AlignmentVertical.Bottom:
				bounds.y = Screen.height - bounds.height;
				break;
			case AlignmentVertical.Center:
				bounds.y = (Screen.height - bounds.height) / 2;
				break;
			}
			bounds.y += verticalOffset;
			
			// context
			GUI.BeginGroup(bounds);
			bounds.x = 0;
			bounds.y = 0;
			
			if (boundType == BoundType.Boxed)
			{
				GUI.Box(bounds, boundingGUIContent, boundingStyle);
			}
			if (useScrollView)
			{
				scrollPosition = GUI.BeginScrollView(contentBounds, scrollPosition, scrollViewRect, alwaysShowHorizontalScrollBar, alwaysShowVerticalScrollBar);
			}
			else
			{
				GUI.BeginGroup(contentBounds);
			}
			DrawGUI(0);
			if (useScrollView)
			{
				GUI.EndScrollView();
			}
			else
			{
				GUI.EndGroup();
			}
			
			GUI.EndGroup();
		}
	}

	protected virtual void DrawFixedWindow(int windowId)
	{
		// context
		GUI.BeginGroup(bounds);
		bounds.x = 0;
		bounds.y = 0;
		
		if (useScrollView)
		{
			scrollPosition = GUI.BeginScrollView(contentBounds, scrollPosition, scrollViewRect, alwaysShowHorizontalScrollBar, alwaysShowVerticalScrollBar);
		}
		else
		{
			GUI.BeginGroup(contentBounds);
		}
		DrawGUI(windowId);
		if (useScrollView)
		{
			GUI.EndScrollView();
		}
		else
		{
			GUI.EndGroup();
		}
		
		GUI.EndGroup();
	}
	
	protected virtual void DrawDraggableWindow(int windowId)
	{
		if (useScrollView)
		{
			scrollPosition = GUI.BeginScrollView(contentBounds, scrollPosition, scrollViewRect, alwaysShowHorizontalScrollBar, alwaysShowVerticalScrollBar);
		}
		else
		{
			GUI.BeginGroup(contentBounds);
		}
		DrawGUI(windowId);
		if (useScrollView)
		{
			GUI.EndScrollView();
		}
		else
		{
			GUI.EndGroup();
		}
		
		GUI.DragWindow(draggableWindowBounds);
	}
	
	protected abstract void DrawGUI(int windowId);

	public Rect Bounds
	{
		get
		{
			if (boundType == BoundType.DraggableWindow)
			{
				return bounds;
			}
			else
			{
				Rect bounds = this.bounds;
				
				// horizontal binding
				switch (horizontalAlignment)
				{
				case AlignmentHorizontal.Right:
					bounds.x = Screen.width - bounds.width;
					break;
				case AlignmentHorizontal.Center:
					bounds.x = (Screen.width - bounds.width) / 2;
					break;
				}
				bounds.x += horizontalOffset;
		
				// vertical binding
				switch (verticalAlignment)
				{
				case AlignmentVertical.Bottom:
					bounds.y = Screen.height - bounds.height;
					break;
				case AlignmentVertical.Center:
					bounds.y = (Screen.height - bounds.height) / 2;
					break;
				}
				bounds.y += verticalOffset;
				
				return bounds;
			}
		}
	}
	
	public int WindowId
	{
		get { return windowId; }
	}
	
	public float ScrollViewContentWidth
	{
		set { scrollViewRect.width = value; }
	}

	public float ScrollViewContentHeight
	{
		set { scrollViewRect.height = value; }
	}
	
}
