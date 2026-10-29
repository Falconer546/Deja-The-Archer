using UnityEngine;

[AddComponentMenu("")]
[ExecuteInEditMode]
public class HierarchyFolder : MonoBehaviour
{

    [ColorUsage(true, true)]
    public Color folderColor = new Color(0.2f, 0.4f, 0.7f, 1f);
    public bool hideTransform = false;

    [Header("Reset")]
    [SerializeField] private bool resetTransform = true;

    [Header("Position Locks")]
    [SerializeField] private bool lockPosX = true;
    [SerializeField] private bool lockPosY = true;
    [SerializeField] private bool lockPosZ = true;

    [Header("Rotation Locks")]
    [SerializeField] private bool lockRotX = true;
    [SerializeField] private bool lockRotY = true;
    [SerializeField] private bool lockRotZ = true;

    [Header("Scale Lock")]
    [SerializeField] private bool lockScale = true;


    private void OnValidate()
    {
        ApplyHideFlags(); 
    }
#if UNITY_EDITOR
    private void Update()
    {
        if (resetTransform)
            if (NeedsReset()) ResetTransform();

    }

    private bool NeedsReset()
    {
        Vector3 p = transform.position;
        if (!Mathf.Approximately(p.x, 0f)) return true;
        if (!Mathf.Approximately(p.y, 0f)) return true;
        if (!Mathf.Approximately(p.z, 0f)) return true;

        Vector3 e = NormalizedEuler();
        if (!Mathf.Approximately(e.x, 0f)) return true;
        if (!Mathf.Approximately(e.y, 0f)) return true;
        if (!Mathf.Approximately(e.z, 0f)) return true;

        if (transform.localScale != Vector3.one) return true;

        return false;
    }

        private Vector3 NormalizedEuler()
    {
        Vector3 raw = transform.eulerAngles;
        return new Vector3(
            Mathf.DeltaAngle(0f, raw.x),
            Mathf.DeltaAngle(0f, raw.y),
            Mathf.DeltaAngle(0f, raw.z));
    }

    private void ResetTransform()
    {
        // Pozice – zamčené osy na 0, ostatní ponecháme
        Vector3 p = transform.position;
        if (lockPosX) p.x = 0f;
        if (lockPosY) p.y = 0f;
        if (lockPosZ) p.z = 0f;
        transform.position = p;

        // Rotace – normalizované euler, zamčené osy na 0
        Vector3 e = NormalizedEuler();
        if (lockRotX) e.x = 0f;
        if (lockRotY) e.y = 0f;
        if (lockRotZ) e.z = 0f;
        transform.rotation = Quaternion.Euler(e);

        // Scale
        if (lockScale) transform.localScale = Vector3.one;
    }
#endif
    private void ApplyHideFlags()
    {
#if UNITY_EDITOR
        if (hideTransform)
        {
            transform.hideFlags |= HideFlags.NotEditable;
            transform.hideFlags |= HideFlags.HideInInspector;
        }
        else
        {
            transform.hideFlags = HideFlags.None;
        }
#else
        transform.hideFlags = HideFlags.None;
#endif
    }
}