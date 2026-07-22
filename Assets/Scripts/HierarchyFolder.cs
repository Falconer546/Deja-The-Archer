using UnityEngine;

[AddComponentMenu("")] // Skryje položku v menu Component
[ExecuteInEditMode]
public class HierarchyFolder : MonoBehaviour
{
#if UNITY_EDITOR
    [ColorUsage(true, true)]
    public Color folderColor = new Color(0.2f, 0.4f, 0.7f, 1f);


    private void OnValidate()
    {
        //ApplyHideFlags();
    }
    private void Update()
    {
        if(transform.position != Vector3.zero || transform.rotation.eulerAngles != Vector3.zero || transform.localScale != Vector3.one)
            ResetTransform();
    }

    private void ResetTransform()
    {
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        transform.localScale = Vector3.one;
    }

    private void ApplyHideFlags()
    {
        transform.hideFlags |= HideFlags.NotEditable;
        transform.hideFlags |= HideFlags.HideInInspector;
    }
#endif
    
}