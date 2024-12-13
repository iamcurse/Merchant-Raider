using UnityEngine;
using UnityEngine.InputSystem;

public static class Utility
{
    public static float AngleTowardsMouse(Vector3 playerPosition)
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        var direction = mousePosition - playerPosition;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}
