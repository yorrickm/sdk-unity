using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class RoarInspector : Editor
{
	public const int STYLE_LOGO = 0;
	
	protected static GUISkin roarInspectorSkin;
	protected static GUISkin inspectorSkin;
	
	protected virtual void OnEnable()
	{
		if (roarInspectorSkin == null)
		{
			string path;
			if (EditorGUIUtility.isProSkin)
				path = "RoarEngine/RoarSDK-dark.guiskin";
			else
				path = "RoarEngine/RoarSDK-light.guiskin";				
			roarInspectorSkin = EditorGUIUtility.Load(path) as GUISkin;
		}

		if (inspectorSkin == null)
		{
			inspectorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
			
			// modify the Box
			if (EditorGUIUtility.isProSkin)
				inspectorSkin.box.normal.textColor = new Color(1, 1, 1, 0.75f);
			else
				inspectorSkin.box.normal.textColor = new Color(0, 0, 0, 0.75f);
		}
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		
		// custom inspector GUI
		DrawGUI();
		
		serializedObject.ApplyModifiedProperties();
		
		// the logo
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(string.Empty, roarInspectorSkin.customStyles[STYLE_LOGO]);
		GUILayout.Space(8);
		GUILayout.EndHorizontal();
	}
	
	protected abstract void DrawGUI();
}
