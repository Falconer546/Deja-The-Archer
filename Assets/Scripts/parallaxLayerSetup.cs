using System;
using Unity.VisualScripting;
using UnityEngine;
/*
[ExecuteInEditMode]
public class parallaxLayerSetup : MonoBehaviour
{
    
    public GlobalGameSettings globalGameSettings;
#if UNITY_EDITOR

    void Update()
    {
        if (globalGameSettings.lockAssetsDepth == true)
            SetZeroZ();
    }

    [ContextMenu("Set Z to Zero")]    
    void SetZeroZ()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform _childTransform = transform.GetChild(i);
            
            _childTransform.localPosition = new Vector3(_childTransform.localPosition.x, _childTransform.localPosition.y, 0); 
        }
    }

#endif
}*/