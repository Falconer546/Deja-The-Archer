using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HierarchyFolder))]
[CanEditMultipleObjects]
public class HierarchyFolderInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty hideTransformProp = serializedObject.FindProperty("hideTransform");
        bool hide = hideTransformProp != null && hideTransformProp.boolValue;

        // Obyčejné property
        DrawProperty("folderColor", "Folder Color");
        DrawProperty("hideTransform", "Hide Transform");

        if (!hide)
        {
            DrawProperty("resetTransform", "Reset Transform");

            EditorGUILayout.Space();

            DrawAxisRow("Lock Position", "lockPosX", "lockPosY", "lockPosZ");
            DrawAxisRow("Lock Rotation", "lockRotX", "lockRotY", "lockRotZ");

            EditorGUILayout.Space();

            DrawProperty("lockScale", "Lock Scale");
        }

        serializedObject.ApplyModifiedProperties();
    }

    // Pomocná metoda pro jeden PropertyField s popiskem
    private void DrawProperty(string path, string label)
    {
        SerializedProperty prop = serializedObject.FindProperty(path);
        if (prop != null)
            EditorGUILayout.PropertyField(prop, new GUIContent(label));
    }

    // Vykreslí tři booly (X, Y, Z) hezky vedle sebe s prefixem
    private void DrawAxisRow(string label, string xPath, string yPath, string zPath)
    {
        SerializedProperty x = serializedObject.FindProperty(xPath);
        SerializedProperty y = serializedObject.FindProperty(yPath);
        SerializedProperty z = serializedObject.FindProperty(zPath);

        if (x == null || y == null || z == null) return;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(label);

        x.boolValue = EditorGUILayout.ToggleLeft("X", x.boolValue, GUILayout.Width(40));
        y.boolValue = EditorGUILayout.ToggleLeft("Y", y.boolValue, GUILayout.Width(40));
        z.boolValue = EditorGUILayout.ToggleLeft("Z", z.boolValue, GUILayout.Width(40));

        EditorGUILayout.EndHorizontal();
    }
}