using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RoarUIController))]
public class RoarUIControllerInspector : RoarUIInspector
{
	private SerializedProperty disableOnAwake;
	private SerializedProperty fullScreenBackground;
	
	private SerializedProperty uiModules;
	
	private RoarUIController roarUIController;
	
	protected override void OnEnable()
	{
		base.OnEnable();
		
		roarUIController = (RoarUIController)target;
		
		disableOnAwake = serializedObject.FindProperty("disableOnAwake");
		fullScreenBackground = serializedObject.FindProperty("fullScreenBackground");
		uiModules = serializedObject.FindProperty("uiModules");
	}
	
	protected override void DrawGUI()
	{
		base.DrawGUI();
		
		// disableOnAwake
		Comment("When set, the Roar user interface will be hidden immediately.");
		EditorGUILayout.PropertyField(disableOnAwake);
		
		// fullScreenBackground
		EditorGUILayout.Space();
		Comment("UI boundary constraint.");
		EditorGUILayout.PropertyField(fullScreenBackground);
		if (!fullScreenBackground.boolValue)
		{
			EditorGUILayout.PropertyField(bounds);
			// constrain
			Rect rect = bounds.rectValue;
			if (rect.width < 0) rect.width = 0;
			if (rect.height < 0) rect.height = 0;
			bounds.rectValue = rect;
		}
		
		if (!Application.isPlaying)
		{
			// login module
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			SerializedProperty loginModuleProperty = (uiModules.arraySize >= (int)RoarUIPanel.Login) ? serializedObject.FindProperty(string.Format("uiModules.Array.data[{0}]", (int)RoarUIPanel.Login)) : null;
			if (loginModuleProperty == null || loginModuleProperty.objectReferenceValue == null)
			{
				if (GUILayout.Button("Add Login Module"))
				{
					// add the login module
					GameObject loginModule = new GameObject("Login");
					loginModule.transform.parent = roarUIController.transform;
					RoarLoginUI roarLoginUI = loginModule.AddComponent<RoarLoginUI>();
					roarLoginUI.uiController = roarUIController;
					roarLoginUI.depth = 1;
					roarLoginUI.parent = roarUIController;
					EditorUtility.SetDirty(roarLoginUI);
					
					if (uiModules.arraySize <= (int)RoarUIPanel.Login)
						uiModules.arraySize++;
					loginModuleProperty = serializedObject.FindProperty(System.String.Format("uiModules.Array.data[{0}]", (int)RoarUIPanel.Login));
					loginModuleProperty.objectReferenceValue = roarLoginUI;
					Selection.activeObject = roarLoginUI;
				}
			}
			else
			{
				if (GUILayout.Button("Remove Login Module"))
				{
					if (EditorUtility.DisplayDialog("Remove Login Module?", "Are you sure that you want to remove the login module?", "Remove", "Cancel"))
					{
						RoarLoginUI roarLoginUI = (RoarLoginUI)loginModuleProperty.objectReferenceValue;
						DestroyImmediate(roarLoginUI.gameObject);
						loginModuleProperty.objectReferenceValue = null;
					}
				}
			}
		}
		else
		{
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			if (GUILayout.Button("Show Login Panel"))
			{
				roarUIController.UIPanel = RoarUIPanel.Login;
			}
			if (roarUIController.enabled && GUILayout.Button("Hide User Interface"))
			{
				roarUIController.UIPanel = RoarUIPanel.Off;
			}
		}
	}
}
