using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameManager g = (GameManager)target;

        if (GUILayout.Button("Update Room values"))
        {
            g.UpdateRoomValues();
        }
    }
}
