using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HierarchyFolder))]
[CanEditMultipleObjects]
public class HierarchyFolderInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty _colorProp = serializedObject.FindProperty("folderColor");
        if (_colorProp != null)
        {
            EditorGUILayout.PropertyField(_colorProp, new GUIContent("Folder Color"));
        }

        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();

        serializedObject.ApplyModifiedProperties();
    }
}