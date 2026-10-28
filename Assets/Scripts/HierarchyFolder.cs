using UnityEngine;

[AddComponentMenu("")]
[ExecuteInEditMode]
public class HierarchyFolder : MonoBehaviour
{

    [ColorUsage(true, true)]
    public Color folderColor = new Color(0.2f, 0.4f, 0.7f, 1f);


    private void OnValidate()
    {
        ApplyHideFlags(); 
    }
#if UNITY_EDITOR
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
#endif
    private void ApplyHideFlags()
    {
#if UNITY_EDITOR
        transform.hideFlags |= HideFlags.NotEditable;
        transform.hideFlags |= HideFlags.HideInInspector;
#else
        transform.hideFlags = HideFlags.None;
#endif
    }
}