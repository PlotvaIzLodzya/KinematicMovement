using System;
using UnityEngine;

public static class Extensions
{
    public static Vector3 RoundFloat(this Vector3 value, int digits = 5)
    {
        value.x = (float)Math.Round(value.x, digits);
        value.y = (float)Math.Round(value.y, digits);
        value.z = (float)Math.Round(value.z, digits);

        return value;
    }

    public static float GetMax(this Vector3 vector)
    {
        return Mathf.Max(vector.x, vector.y, vector.z);
    }

    public static Vector3 RotatePointAroundPivot(this Vector3 point, Vector3 pivot, Quaternion angle)
    {
        return angle * (point - pivot) + pivot;
    }

    public static HitInfo ToHitInfo(this RaycastHit2D hit, Collider2D selfCollider)
    {
        var dist = hit.distance;
        bool haveHit = hit.collider != null;
        var collDist = 0f;
        if (haveHit)
        {
            var d = hit.collider.Distance(selfCollider);
            collDist = d.distance;
        }
        var hitInfo = new HitInfo()
        {
            Point = hit.point,
            Normal = hit.normal,
            Distance = dist,
            HaveHit = haveHit,
            Transform = hit.transform,
            ColliderDistance = collDist,
        };

        return hitInfo;
    }

    public static HitInfo ToHitInfo(this RaycastHit hit)
    {
        var hitInfo = new HitInfo()
        {
            Point = hit.point,
            Normal = hit.normal,
            Distance = hit.distance,
            ColliderDistance = hit.distance + MovementConfig.ContactOffset,
            HaveHit = hit.collider != null,
            Transform = hit.transform
        };

        return hitInfo;
    }

}
