using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerGUI : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameManager g = (GameManager)target;
    }
}
