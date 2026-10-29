using UnityEngine;
#if UNITY_EDITOR
[ExecuteInEditMode]
public class ResetChildTransform : MonoBehaviour
{
    [Header("Position")]
    [SerializeField] private bool lockPosX = false;
    [SerializeField] private bool lockPosY = false;
    [SerializeField] private bool lockPosZ = false;

    [Header("Rotation")]
    [SerializeField] private bool lockRotX = false;
    [SerializeField] private bool lockRotY = false;
    [SerializeField] private bool lockRotZ = false;

    [Header("Scale")]
    [SerializeField] private bool lockScaleX = false;
    [SerializeField] private bool lockScaleY = false;
    [SerializeField] private bool lockScaleZ = false;


    private void LateUpdate()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            // ---- Position ----
            Vector3 pos = child.localPosition;

            if (lockPosX && pos.x != 0f) pos.x = 0f;
            if (lockPosY && pos.y != 0f) pos.y = 0f;
            if (lockPosZ && pos.z != 0f) pos.z = 0f;

            child.localPosition = pos;

            // ---- Rotation ----
            Vector3 rot = child.localEulerAngles;

            if (lockRotX && rot.x != 0f) rot.x = 0f;
            if (lockRotY && rot.y != 0f) rot.y = 0f;
            if (lockRotZ && rot.z != 0f) rot.z = 0f;

            child.localEulerAngles = rot;

            // ---- Scale ----
            Vector3 scale = child.localScale;

            if (lockScaleX && scale.x != 1f) scale.x = 1f;
            if (lockScaleY && scale.y != 1f) scale.y = 1f;
            if (lockScaleZ && scale.z != 1f) scale.z = 1f;

            child.localScale = scale;
        }
    }
}
#endif