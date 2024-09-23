using UnityEngine;
using UnityEditor;

namespace Fix
{
    [UnityEditor.CustomEditor(typeof(UIPanel))]
    public class UIPanelInspector : Editor
    {
		public override void OnInspectorGUI()
		{
			GUI.enabled = Application.isPlaying; 
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("ResetPosition"))
			{
				Instance.Reset();
			}
	        if (GUILayout.Button("StartAnimation"))
	        {
		        Instance.StartAnimation();
	        }
			GUILayout.EndHorizontal();
			GUI.enabled = true;
			DrawDefaultInspector();
		}
		UIPanel Instance{
			get{return (UIPanel)target; }
		}
    }

}

