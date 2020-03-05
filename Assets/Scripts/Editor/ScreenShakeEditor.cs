using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScreenShake))]
public class ScreenShakeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ScreenShake sc = (ScreenShake)target;

        if (GUILayout.Button("Shake Low"))
        {
            sc.TriggerShake(0.5f, ScreenShake.ShakeIntensity.low);
        }
        if (GUILayout.Button("Shake Medium"))
        {
            sc.TriggerShake(0.5f, ScreenShake.ShakeIntensity.medium);
        }
        if (GUILayout.Button("Shake High"))
        {
            sc.TriggerShake(0.5f, ScreenShake.ShakeIntensity.high);
        }
    }
}
