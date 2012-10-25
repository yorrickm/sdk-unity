using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RoarModuleController))]
public class RoarModuleControllerInspector : RoarModuleInspector
{
	private SerializedProperty disableOnAwake;
	private SerializedProperty fullScreenBackground;
	
	private SerializedProperty uiModules;
	
	private RoarModuleController roarModuleController;
	private RoarModulePanel displayedPanel = RoarModulePanel.Off;
	private bool[] uiModulesEnabled;
	
	private static bool[] MODULES_USABLE = new bool[]
	{
		true,
		false,
		true,
		false,
		false,
		false,
		false,
		false,
		false,
	};
	
	protected override void OnEnable()
	{
		base.OnEnable();
		
		roarModuleController = (RoarModuleController)target;
		
		disableOnAwake = serializedObject.FindProperty("disableOnAwake");
		fullScreenBackground = serializedObject.FindProperty("fullScreenBackground");
		uiModules = serializedObject.FindProperty("uiModules");
		
		uiModulesEnabled = new bool[System.Enum.GetValues(typeof(RoarModulePanel)).Length - 1];
		for (int i=0; i<uiModulesEnabled.Length; i++)
		{
			SerializedProperty moduleProperty = (uiModules.arraySize >= i) ? serializedObject.FindProperty(string.Format("uiModules.Array.data[{0}]", i)) : null;
			uiModulesEnabled[i] = moduleProperty != null && moduleProperty.objectReferenceValue != null;
		}
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
			// module enablers and disablers
			
			Comment("Roar component modules to enable for use within the game.");
			foreach (RoarModulePanel module in System.Enum.GetValues(typeof(RoarModulePanel)))
			{
				if (module != RoarModulePanel.Off && MODULES_USABLE[(int)module])
				{
					ModuleEnablerUI(module);
				}
			}
		}
		else
		{
			// module selector (runtime in editor)
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			displayedPanel = (RoarModulePanel)EditorGUILayout.EnumPopup(new GUIContent("Active Module"), displayedPanel);
			SerializedProperty module = (uiModules.arraySize >= (int)displayedPanel) ? serializedObject.FindProperty(string.Format("uiModules.Array.data[{0}]", (int)displayedPanel)) : null;
			bool hasTheModule = displayedPanel == RoarModulePanel.Off || (module != null && module.objectReferenceValue != null);
			GUI.enabled = hasTheModule;
			if (GUILayout.Button("OK", GUILayout.Width(32), GUILayout.Height(14)))
			{
				roarModuleController.CurrentModulePanel = displayedPanel;
			}
			GUI.enabled = true;
			EditorGUILayout.EndHorizontal();			
		}
	}
	
	private void ModuleEnablerUI(RoarModulePanel module)
	{
		bool moduleEnableState = EditorGUILayout.Toggle(new GUIContent(string.Format("{0}", module)), uiModulesEnabled[(int)module]);
		if (moduleEnableState != uiModulesEnabled[(int)module])
		{
			SerializedProperty moduleProperty = (uiModules.arraySize >= (int)module) ? serializedObject.FindProperty(string.Format("uiModules.Array.data[{0}]", (int)module)) : null;
			
			if (moduleEnableState && (moduleProperty == null || moduleProperty.objectReferenceValue == null)) // add
			{
				// add the module
				GameObject moduleGO = new GameObject(module.ToString());
				moduleGO.transform.parent = roarModuleController.transform;
				RoarModule roarModule = moduleGO.AddComponent(string.Format("Roar{0}Module", module)) as RoarModule;
				roarModule.ResetToDefaultConfiguration();
				roarModule.uiController = roarModuleController;
				roarModule.parent = roarModuleController;
				EditorUtility.SetDirty(moduleGO);
				
				// grow the uiModules array if necessary
				if (uiModules.arraySize <= (int)module)
				{
					int currentSize = uiModules.arraySize;
					uiModules.arraySize = (int)module + 1;
					for (int i=currentSize; i<uiModules.arraySize; i++)
					{
						moduleProperty = serializedObject.FindProperty(System.String.Format("uiModules.Array.data[{0}]", i));
						moduleProperty.objectReferenceValue = null;
					}
				}
				
				moduleProperty = serializedObject.FindProperty(System.String.Format("uiModules.Array.data[{0}]", (int)module));
				moduleProperty.objectReferenceValue = roarModule;
				uiModulesEnabled[(int)module] = true;
				Selection.activeObject = roarModule;
			}
			else // remove
			{
				if (EditorUtility.DisplayDialog(string.Format("Remove {0} Module?", module), string.Format("Are you sure that you want to remove the {0} module?", module), "Remove", "Cancel"))
				{
					RoarModule roarModule = (RoarModule)moduleProperty.objectReferenceValue;
					DestroyImmediate(roarModule.gameObject);
					moduleProperty.objectReferenceValue = null;
					uiModulesEnabled[(int)module] = false;
				}
			}
		}
	}
	
}
