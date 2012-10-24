using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class RoarUIWidgetInspector : RoarInspector
{
	protected SerializedProperty customGUISkin;
	protected SerializedProperty depth;
	protected SerializedProperty bounds;
	protected SerializedProperty color;
	protected SerializedProperty boundType;
	protected SerializedProperty boundingStyle;
	protected SerializedProperty boundingTitle;
	protected SerializedProperty boundingImage;
	protected SerializedProperty draggableWindowFullScreen;
	protected SerializedProperty draggableWindowBounds;
	protected SerializedProperty horizontalAlignment;
	protected SerializedProperty verticalAlignment;
	protected SerializedProperty horizontalOffset;
	protected SerializedProperty verticalOffset;
	protected SerializedProperty useScrollView;
	protected SerializedProperty initialContentWidth;
	protected SerializedProperty initialContentHeight;
	protected SerializedProperty alwaysShowHorizontalScrollBar;
	protected SerializedProperty alwaysShowVerticalScrollBar;
	
	protected override void OnEnable()
	{
		base.OnEnable ();
		
		customGUISkin = serializedObject.FindProperty("customGUISkin");
		depth = serializedObject.FindProperty("depth");
		bounds = serializedObject.FindProperty("bounds");
		color = serializedObject.FindProperty("color");
		boundType = serializedObject.FindProperty("boundType");
		boundingStyle = serializedObject.FindProperty("boundingStyle");
		boundingTitle = serializedObject.FindProperty("boundingTitle");
		boundingImage = serializedObject.FindProperty("boundingImage");
		draggableWindowFullScreen = serializedObject.FindProperty("draggableWindowFullScreen");
		draggableWindowBounds = serializedObject.FindProperty("draggableWindowBounds");
		horizontalAlignment = serializedObject.FindProperty("horizontalAlignment");
		verticalAlignment = serializedObject.FindProperty("verticalAlignment");
		horizontalOffset = serializedObject.FindProperty("horizontalOffset");
		verticalOffset = serializedObject.FindProperty("verticalOffset");
		useScrollView = serializedObject.FindProperty("useScrollView");
		initialContentWidth = serializedObject.FindProperty("initialContentWidth");
		initialContentHeight = serializedObject.FindProperty("initialContentHeight");
		alwaysShowHorizontalScrollBar = serializedObject.FindProperty("alwaysShowHorizontalScrollBar");
		alwaysShowVerticalScrollBar = serializedObject.FindProperty("alwaysShowVerticalScrollBar");
	}

	protected override void DrawGUI()
	{
		// customGUISkin
		EditorGUILayout.Space();
		Comment("You can specify custom skin for the user interface. Otherwise, the default skin will be used.");
		EditorGUILayout.PropertyField(customGUISkin);
		
		// rendering properties
		Comment("Widget rendering properties.");
		EditorGUILayout.PropertyField(depth, new GUIContent("Draw Order"));		
		EditorGUILayout.PropertyField(color);
		EditorGUILayout.PropertyField(horizontalAlignment);
		EditorGUILayout.PropertyField(horizontalOffset);
		EditorGUILayout.PropertyField(verticalAlignment);
		EditorGUILayout.PropertyField(verticalOffset);
		
		// boundary properties
		Comment("Draw boundary properties.");
		EditorGUILayout.PropertyField(boundType, new GUIContent("Type"));
		EditorGUILayout.PropertyField(bounds, new GUIContent("Render Bounds"));
		if (bounds.rectValue.width <= 0 || bounds.rectValue.height <= 0)
		{
			EditorGUILayout.HelpBox("Since the render bounds width and/or height is 0, nothing will be visible.", MessageType.Warning);
		}
		EditorGUILayout.Space();
		if (boundType.enumValueIndex == 3) // DraggableWindow
		{
			EditorGUILayout.PropertyField(draggableWindowFullScreen, new GUIContent("Draggable Fullscreen"));
			if (!draggableWindowFullScreen.boolValue)
			{
				EditorGUILayout.PropertyField(draggableWindowBounds, new GUIContent("Draggable Bounds"));
				EditorGUILayout.Space();
			}
		}
		EditorGUILayout.PropertyField(boundingStyle, new GUIContent("Style"));
		EditorGUILayout.PropertyField(boundingTitle, new GUIContent("Title"));
		EditorGUILayout.PropertyField(boundingImage, new GUIContent("Image"));
		EditorGUILayout.PropertyField(useScrollView, new GUIContent("Enable Scrolling"));	
		if (useScrollView.boolValue)
		{
			EditorGUILayout.PropertyField(initialContentWidth);
			EditorGUILayout.PropertyField(initialContentHeight);
			EditorGUILayout.PropertyField(alwaysShowHorizontalScrollBar, new GUIContent("Always Show Horiz. Bar"));
			EditorGUILayout.PropertyField(alwaysShowVerticalScrollBar, new GUIContent("Always Show Vert. Bar"));
		}
	}
}
