using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResetChildTransform))]
[CanEditMultipleObjects] // Tohle už tam máš, zajišťuje multi-edit
public class ResetChildTransformInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawAxisRow("Lock Position", "lockPosX", "lockPosY", "lockPosZ");
        DrawAxisRow("Lock Rotation", "lockRotX", "lockRotY", "lockRotZ");
        DrawAxisRow("Lock Scale",    "lockScaleX", "lockScaleY", "lockScaleZ");

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawAxisRow(string label, string xPath, string yPath, string zPath)
    {
        SerializedProperty x = serializedObject.FindProperty(xPath);
        SerializedProperty y = serializedObject.FindProperty(yPath);
        SerializedProperty z = serializedObject.FindProperty(zPath);

        if (x == null || y == null || z == null) return;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(label);

        DrawToggle(x, "X");
        DrawToggle(y, "Y");
        DrawToggle(z, "Z");

        EditorGUILayout.EndHorizontal();
    }

    // Pomocná metoda, která správně ošetří multi-edit
    private void DrawToggle(SerializedProperty prop, string label)
    {
        // 1. Zjistíme, zda mají vybrané objekty různou hodnotu (zobrazí se pomlčka)
        EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;

        // 2. Sledujeme, zda uživatel kliknul
        EditorGUI.BeginChangeCheck();
        bool newValue = EditorGUILayout.ToggleLeft(label, prop.boolValue, GUILayout.Width(40));
        if (EditorGUI.EndChangeCheck())
        {
            // 3. Pokud ano, zapíšeme to do všech vybraných objektů
            prop.boolValue = newValue;
        }

        // 4. Resetujeme mixed state pro další prvky
        EditorGUI.showMixedValue = false;
    }
}