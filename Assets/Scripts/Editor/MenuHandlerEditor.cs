using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MenuHandler))]
public class MenuHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Reset Playerprefs"))
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
