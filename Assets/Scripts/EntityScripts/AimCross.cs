using UnityEngine;
using UnityEngine.InputSystem;

public class AimCross : MonoBehaviour
{

    private InputAction lookAction;
    public Vector3 crossPosition;
    public Vector2 _currentOffset;
    public float aimSpeed = 0.001f, crossDistance = 2f;

    void Awake()
    {
        lookAction = InputSystem.actions.FindAction("Look");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 _input = lookAction.ReadValue<Vector2>();
        _currentOffset += _input * aimSpeed;
        
        _currentOffset = _currentOffset.normalized * crossDistance;

        transform.localPosition = new Vector3(_currentOffset.x, _currentOffset.y, 0);
    }
}
