using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class HierarchyFolderEditor
{
    private static Texture2D _gradientTexture;
    private static Color _lastColor;

    static HierarchyFolderEditor()
    {
        EditorApplication.hierarchyWindowItemByEntityIdOnGUI += OnHierarchyItemGUI;
    }

    private static void CreateGradientTexture(Color color)
    {
        if (_gradientTexture == null || _lastColor != color)
        {
            _gradientTexture = new Texture2D(3, 1, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp
            };
            Color left = new Color(color.r, color.g, color.b, 0.25f);
            Color right = new Color(color.r, color.g, color.b, 0f);
            _gradientTexture.SetPixel(0, 0, left);
            _gradientTexture.SetPixel(1, 0, left);
            _gradientTexture.SetPixel(2, 0, right);
            _gradientTexture.Apply();
            _lastColor = color;
        }
    }

    private static void OnHierarchyItemGUI(EntityId entityId, Rect selectionRect)
    {
        GameObject obj = EditorUtility.EntityIdToObject(entityId) as GameObject;
        if (obj == null) return;

        HierarchyFolder folder = obj.GetComponent<HierarchyFolder>();
        if (folder == null) return;

        // Vytvoříme texturu s aktuální barvou (pouze pokud se změnila)
        CreateGradientTexture(folder.folderColor);

        // ---- Kreslení pozadí s gradientem ----
        Rect bgRect = selectionRect;
        bgRect.x = 0;
        bgRect.width = selectionRect.x + selectionRect.width;
        GUI.DrawTexture(bgRect, _gradientTexture, ScaleMode.StretchToFill, true);

        // ---- Ikona složky ----
        Rect iconRect = new Rect(selectionRect.x + 2, selectionRect.y + 2, 16, 16);
        GUI.DrawTexture(iconRect, EditorGUIUtility.IconContent("Folder Icon").image);

        // ---- Vzhled názvu ----
        GUI.color = new Color(0.8f, 0.9f, 1f);
        GUI.skin.label.fontStyle = FontStyle.Bold;
    }

    [MenuItem("GameObject/Hierarchy Folder", false, 10)]
    private static void CreateFolder(MenuCommand menuCommand)
    {
        GameObject go = new GameObject("--- Nová složka ---");
        HierarchyFolder folder = go.AddComponent<HierarchyFolder>();
        // Nastavíme výchozí barvu (např. modrou)
        folder.folderColor = new Color(0.2f, 0.4f, 0.7f, 1f);
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}