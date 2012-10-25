using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class RoarModuleInspector : RoarInspector
{
	protected SerializedProperty customGUISkin;
	protected SerializedProperty constrainToBounds;
	protected SerializedProperty bounds;
	protected SerializedProperty horizontalContentAlignment;
	protected SerializedProperty verticalContentAlignment;
	protected SerializedProperty horizontalContentOffset;
	protected SerializedProperty verticalContentOffset;
	
	// background properties
	protected SerializedProperty backgroundType;
	protected SerializedProperty backgroundColor;
	protected SerializedProperty backgroundImage;
	protected SerializedProperty backgroundSolidColorStyle;
	protected SerializedProperty backgroundImageStyle;
	protected SerializedProperty extendedBackgroundWidth;
	protected SerializedProperty extendedBackgroundHeight;
	
	protected bool showBoundsConstraintSettings = true;
	protected bool showAlignmentConstraintSettings = true;
	
	protected override void OnEnable()
	{
		base.OnEnable();
		customGUISkin = serializedObject.FindProperty("customGUISkin");
		constrainToBounds = serializedObject.FindProperty("constrainToBounds");
		bounds = serializedObject.FindProperty("bounds");
		horizontalContentAlignment = serializedObject.FindProperty("horizontalContentAlignment");
		verticalContentAlignment = serializedObject.FindProperty("verticalContentAlignment");
		horizontalContentOffset = serializedObject.FindProperty("horizontalContentOffset");
		verticalContentOffset = serializedObject.FindProperty("verticalContentOffset");
		backgroundType = serializedObject.FindProperty("backgroundType");
		backgroundColor = serializedObject.FindProperty("backgroundColor");
		backgroundImage = serializedObject.FindProperty("backgroundImage");
		backgroundSolidColorStyle = serializedObject.FindProperty("backgroundSolidColorStyle");
		backgroundImageStyle = serializedObject.FindProperty("backgroundImageStyle");
		extendedBackgroundWidth = serializedObject.FindProperty("extendedBackgroundWidth");
		extendedBackgroundHeight = serializedObject.FindProperty("extendedBackgroundHeight");
	}

	protected override void DrawGUI()
	{
		// customGUISkin
		EditorGUILayout.Space();
		Comment("You can specify custom skin for the user interface. Otherwise, the default skin will be used.");
		EditorGUILayout.PropertyField(customGUISkin);
		
		// constrainToBounds
		bool isController = (this is RoarModuleControllerInspector);
		if (!isController)
		{
			if (showBoundsConstraintSettings || showAlignmentConstraintSettings)
				Comment("UI containment and alignment constraints.");
			
			if (showBoundsConstraintSettings)
			{
				EditorGUILayout.PropertyField(constrainToBounds);
				if (constrainToBounds.boolValue)
				{
					EditorGUILayout.PropertyField(bounds);
					Rect rect = bounds.rectValue;
					if (rect.width < 0) rect.width = 0;
					if (rect.height < 0) rect.height = 0;
					bounds.rectValue = rect;
					if (showAlignmentConstraintSettings)
						EditorGUILayout.Space();
				}
			}
			
			if (showAlignmentConstraintSettings)
			{
				EditorGUILayout.PropertyField(horizontalContentAlignment, new GUIContent("Horizontal Alignment"));
				EditorGUILayout.PropertyField(horizontalContentOffset, new GUIContent("Horizontal Offset"));
				EditorGUILayout.PropertyField(verticalContentAlignment, new GUIContent("Vertical Alignment"));
				EditorGUILayout.PropertyField(verticalContentOffset, new GUIContent("Vertical Offset"));
			}
		}
		
		// background
		EditorGUILayout.Space();
		Comment("Background properties.");
		EditorGUILayout.PropertyField(backgroundType);
		switch (backgroundType.enumValueIndex)
		{
		case 1: // SolidColor
			EditorGUILayout.PropertyField(backgroundSolidColorStyle, new GUIContent("Style"));
			EditorGUILayout.PropertyField(backgroundColor, new GUIContent("Color"));
			backgroundImage.objectReferenceValue = null;
			break;
		case 2: // Image
			EditorGUILayout.PropertyField(backgroundImage);
			if (backgroundImage.objectReferenceValue == null)
				EditorGUILayout.HelpBox("Please set a background image.", MessageType.Warning);
			break;
		case 3: // ExtentedImage
			EditorGUILayout.PropertyField(backgroundImageStyle, new GUIContent("Style"));
			EditorGUILayout.PropertyField(backgroundColor, new GUIContent("Color"));
			EditorGUILayout.PropertyField(extendedBackgroundWidth, new GUIContent("Width"));
			EditorGUILayout.PropertyField(extendedBackgroundHeight, new GUIContent("Height"));
			backgroundImage.objectReferenceValue = null;
			break;
		default:
			backgroundImage.objectReferenceValue = null;
			break;
		}
	}
}
