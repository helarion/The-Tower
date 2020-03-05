using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Card c = (Card)target;

        if (GUILayout.Button("Flip"))
        {
            c.Flip();
        }
    }
}
