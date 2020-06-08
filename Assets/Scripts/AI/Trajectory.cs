using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory 
{
    const float g = 9.81f;
    private static float GravityDisplacement(float t)
    {
        return (1f/2f)*g*(t*t);
    }

    /* NOT mathematically accurate, but good enough for high velocity */
    public static Vector3 Calculate(Vector3 target, float projectileSpeed)
    {
        float t = target.magnitude/projectileSpeed;
        float d = GravityDisplacement(t);

        Vector3 trajectory = target + Vector3.up*d;
        return trajectory;
    }
}
