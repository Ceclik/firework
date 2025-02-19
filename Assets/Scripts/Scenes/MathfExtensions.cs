using UnityEngine;

public static class MathfExtensions
{
    private const float TAU = Mathf.PI * 2;

    public static float Tau(this Mathf mathf)
    {
        return 2 * Mathf.PI;
    }
}