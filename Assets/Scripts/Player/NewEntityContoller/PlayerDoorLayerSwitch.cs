using System;
using UnityEngine;

/// <summary>
/// Moves the player between depth layers at a door (formerly MoveEntityInLayer).
/// Independent of the state machines: it works in any state.
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class PlayerDoorLayerSwitch : MonoBehaviour
{
    [SerializeField] private float useCooldown = 0.5f;
    [SerializeField] private float zTolerance = 0.16f;   // max |door.z - player.z| to be usable
    [SerializeField] private float zStep = 0.2f;
    [SerializeField] private int sortingLayerStep = 2;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private ICharacterIntent _intent;
    private float _nextUseTime;

    private void Awake()
    {
        _intent = GetComponent<ICharacterIntent>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (DoorMechanics.useableDoor == null) return;
        if (_intent.Move.y <= 0f || Time.time < _nextUseTime) return;

        float doorToPlayerZ = DoorMechanics.useableDoor.transform.position.z - transform.position.z;
        if (Mathf.Abs(doorToPlayerZ) >= zTolerance) return;

        Vector3 pos = transform.position;

        if (doorToPlayerZ < 0f)
        {
            pos.z -= zStep;
            ShiftSortingLayer(+sortingLayerStep);
        }
        else if (doorToPlayerZ > 0f)
        {
            pos.z += zStep;
            ShiftSortingLayer(-sortingLayerStep);
        }
        else
        {
            return;
        }

        transform.position = pos;
        _nextUseTime = Time.time + useCooldown;
    }

    private void ShiftSortingLayer(int delta)
    {
        SortingLayer[] layers = SortingLayer.layers;
        int current = Array.FindIndex(layers, l => l.id == spriteRenderer.sortingLayerID);
        if (current < 0) return;

        int target = Mathf.Clamp(current + delta, 0, layers.Length - 1);
        spriteRenderer.sortingLayerID = layers[target].id;
    }
}
