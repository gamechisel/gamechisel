using UnityEngine;

public static class DrawRays
{
    public static bool enableRays = true;

    public static Color colliderColor = Color.green;
    public static Color hitboxColor = Color.green;
    public static Color physicsColor = Color.blue;
    public static Color inputColor = Color.red;
    public static Color scanColor = Color.yellow;

    public static void DrawRay(Vector3 origin, Vector3 direction, Color color)
    {
        if (enableRays)
        {
            Debug.DrawRay(origin, direction, color);
        }
    }
}
