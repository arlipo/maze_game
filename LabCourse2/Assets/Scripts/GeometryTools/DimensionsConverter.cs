using UnityEngine;

public static class DimensionsConverter {
    public static Vector3 To3Dpos(Vector2 vector2, float y) => new Vector3(vector2.x, y, vector2.y);
    public static Vector2 To2Dpos(Vector3 vector3) => new Vector2(vector3.x, vector3.z);
}