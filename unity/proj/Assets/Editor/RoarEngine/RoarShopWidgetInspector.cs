using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RoarShopWidget))]
public class RoarShopWidgetInspector : RoarUIWidgetInspector
{
	protected override void OnEnable ()
	{
		base.OnEnable ();
	}
	
	protected override void DrawGUI()
	{
		base.DrawGUI();
	}
}
